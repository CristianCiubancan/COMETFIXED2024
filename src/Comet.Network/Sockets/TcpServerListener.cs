// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - TcpServerListener.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Comet.Network.Sockets
{
    /// <summary>
    ///     TcpServerListener implements an asynchronous TCP streaming socket server for high
    ///     performance server logic. Socket operations are processed in value tasks using
    ///     Socket Task Extensions. Inherits from a base class for providing socket operation
    ///     event handling to the non-abstract derived class of TcpServerListener.
    /// </summary>
    /// <typeparam name="TActor">Type of actor passed by the parent project</typeparam>
    public abstract class TcpServerListener<TActor> : TcpServerEvents<TActor>
        where TActor : TcpServerActor
    {
        // Constants
        private const int ReceiveTimeoutSeconds = 30;

        // Fields and properties
        private readonly Semaphore AcceptanceSemaphore;
        private readonly ConcurrentStack<Memory<byte>> BufferPool;
        private readonly bool EnableKeyExchange;
        private readonly int FooterLength;
        private readonly TaskFactory ReceiveTasks;
        private readonly CancellationTokenSource ShutdownToken;
        private readonly Socket Socket;
        private TcpServerRegistry Registry;

        /// <summary>
        ///     Instantiates a new instance of <see cref="TcpServerListener" /> with a new server
        ///     socket for accepting remote or local client connections. Creates preallocated
        ///     buffers for receiving data from clients without expensive allocations per receive
        ///     operation.
        /// </summary>
        /// <param name="maxConn">Maximum number of clients connected</param>
        /// <param name="bufferSize">Preallocated buffer size in bytes</param>
        /// <param name="delay">Use Nagel's algorithm to delay sending smaller packets</param>
        /// <param name="exchange">Use a key exchange before receiving packets</param>
        /// <param name="footerLength">Length of the packet footer</param>
        protected TcpServerListener(
            int maxConn = 1000,
            int bufferSize = 4096,
            bool delay = false,
            bool exchange = false,
            int footerLength = 0)
        {
            // Initialize and configure server socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.LingerState = new LingerOption(false, 0);
            Socket.NoDelay = !delay;
            ShutdownToken = new CancellationTokenSource();

            // Initialize management mechanisms
            AcceptanceSemaphore = new Semaphore(maxConn, maxConn);
            BufferPool = new ConcurrentStack<Memory<byte>>();
            EnableKeyExchange = exchange;
            FooterLength = footerLength;
            ReceiveTasks = new TaskFactory(ShutdownToken.Token);
            Registry = new TcpServerRegistry();

            // Initialize preallocated buffer pool
            for (int i = 0; i < maxConn; i++)
                BufferPool.Push(new Memory<byte>(new byte[bufferSize]));
        }

        /// <summary>
        ///     Binds the server listener to a port and network interface. Specify "0.0.0.0"
        ///     as the endpoint address to bind to all interfaces on the host machine. Starts
        ///     the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <param name="backlog">Maximum connections backlogged for acceptance</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public async Task StartAsync(int port, string address = "0.0.0.0", int backlog = 100)
        {
            Socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Socket.Listen(backlog);

            // Start the background registry cleaner and accepting clients
            await Registry.StartAsync(ShutdownToken.Token);
            await AcceptingAsync();
        }

        /// <summary>
        ///     Accepting accepts client connections asynchronously as a new task. As a client
        ///     connection is accepted, it will be associated with a preallocated buffer and
        ///     a receive task. The accepted socket event will be called after accept.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task AcceptingAsync()
        {
            while (Socket.IsBound && !ShutdownToken.IsCancellationRequested)
                // Block if the maximum connections has been reached. Holds all connection
                // attempts in the backlog of the server socket until a client disconnects
                // and a new client can be accepted. Check shutdown every 5 seconds.
                if (AcceptanceSemaphore.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    // Pop a preallocated buffer and check the connection
                    var socket = await Socket.AcceptAsync();
                    var ip = (socket.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString();
                    if (!Registry.AddActiveClient(ip))
                    {
                        socket.Disconnect(false);
                        AcceptanceSemaphore.Release();
                        continue;
                    }

                    // Construct the client before receiving data
                    BufferPool.TryPop(out var buffer);
                    var actor = await AcceptedAsync(socket, buffer);

                    // Start receiving data from the client connection
                    if (EnableKeyExchange)
                    {
                        var task = ReceiveTasks
                            .StartNew(ExchangingAsync, actor, ShutdownToken.Token)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        var task = ReceiveTasks
                            .StartNew(ReceivingAsync, actor, ShutdownToken.Token)
                            .ConfigureAwait(false);
                    }
                }
        }

        /// <summary>
        ///     Exchanging receives bytes from the accepted client socket when bytes become
        ///     available as a raw buffer of bytes. This method is called once and then invokes
        ///     <see cref="ReceivingAsync" />.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ExchangingAsync(object state)
        {
            // Initialize multiple receive variables
            var actor = state as TActor;
            var timeout = new CancellationTokenSource();
            int consumed = 0, examined = 0, remaining = 0;

            if (actor.Socket.Connected && !ShutdownToken.IsCancellationRequested)
            {
                try
                {
#if !DEBUG
                    using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                        timeout.Token, ShutdownToken.Token);
                    // Receive data from the client socket
                    var receiveOperation = actor.Socket.ReceiveAsync(
                        actor.Buffer.Slice(0),
                        SocketFlags.None,
                        cancellation.Token);

                    timeout.CancelAfter(TimeSpan.FromSeconds(ReceiveTimeoutSeconds));
#else 
                    var receiveOperation = actor.Socket.ReceiveAsync(
                        actor.Buffer[..],
                        SocketFlags.None);
#endif
                    examined = await receiveOperation;
                    if (examined < 9) throw new Exception("Invalid length");
                }
                catch (Exception e)
                {
                    if (e is SocketException socketEx)
                        if (socketEx.SocketErrorCode < SocketError.ConnectionAborted ||
                            socketEx.SocketErrorCode > SocketError.Shutdown)
                            Console.WriteLine(e);

                    actor.Disconnect();
                    Disconnecting(actor);
                    return;
                }

                // Decrypt traffic by first discarding the first 7 bytes, as per TQ Digital's
                // exchange protocol, then decrypting only what is necessary for the exchange.
                // This is to prevent the next packet from being decrypted with the wrong key.
                actor.Cipher?.Decrypt(
                    actor.Buffer.Slice(0, 9).Span,
                    actor.Buffer.Slice(0, 9).Span);
                consumed = BitConverter.ToUInt16(actor.Buffer.Span.Slice(7, 2)) + 7;
                if (consumed > examined)
                {
                    actor.Disconnect();
                    Disconnecting(actor);
                    return;
                }

                actor.Cipher?.Decrypt(
                    actor.Buffer[9..consumed].Span,
                    actor.Buffer[9..consumed].Span);

                // Process the exchange now that bytes are decrypted
                if (!Exchanged(actor, actor.Buffer.Slice(0, consumed).Span))
                {
                    actor.Disconnect();
                    Disconnecting(actor);
                    return;
                }

                // Now that the key has changed, decrypt the rest of the bytes in the buffer
                // and prepare to start receiving packets on a standard receive loop.
                if (consumed < examined)
                {
                    actor.Cipher?.Decrypt(
                        actor.Buffer.Slice(consumed, examined - consumed).Span,
                        actor.Buffer.Slice(consumed, examined - consumed).Span);

                    if (!Splitting(actor, examined, ref consumed))
                    {
                        actor.Disconnect();
                        Disconnecting(actor);
                        return;
                    }

                    remaining = examined - consumed;
                    actor.Buffer.Slice(consumed, examined - consumed).CopyTo(actor.Buffer);
                }
            }

            // Start receiving packets
            await ReceivingAsync(state, remaining);
        }

        /// <summary>
        ///     Receiving receives bytes from the accepted client socket when bytes become
        ///     available. While the client is connected and the server hasn't issued the
        ///     shutdown signal, bytes will be received in a loop.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private Task ReceivingAsync(object state)
        {
            return ReceivingAsync(state, 0);
        }

        /// <summary>
        ///     Receiving receives bytes from the accepted client socket when bytes become
        ///     available. While the client is connected and the server hasn't issued the
        ///     shutdown signal, bytes will be received in a loop.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <param name="remaining">Starting offset to receive bytes to</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ReceivingAsync(object state, int remaining)
        {
            // Initialize multiple receive variables
            var actor = state as TActor;
            var timeout = new CancellationTokenSource();
            int examined = 0, consumed = 0;

            while (actor.Socket.Connected && !ShutdownToken.IsCancellationRequested)
            {
                try
                {
                    using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                        timeout.Token, ShutdownToken.Token);
                    // Receive data from the client socket
                    var receiveOperation = actor.Socket.ReceiveAsync(
                        actor.Buffer.Slice(remaining),
                        SocketFlags.None,
                        cancellation.Token);

                    timeout.CancelAfter(TimeSpan.FromSeconds(ReceiveTimeoutSeconds));
                    examined = await receiveOperation;
                    if (examined == 0) break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode < SocketError.ConnectionAborted ||
                        e.SocketErrorCode > SocketError.Shutdown)
                        Console.WriteLine(e);
                    break;
                }

                // Decrypt traffic
                actor.Cipher?.Decrypt(
                    actor.Buffer.Slice(remaining, examined).Span,
                    actor.Buffer.Slice(remaining, examined).Span);

                // Handle splitting and processing of data
                consumed = 0;
                if (!Splitting(actor, examined + remaining, ref consumed))
                {
                    actor.Disconnect();
                    break;
                }

                remaining = examined + remaining - consumed;
                actor.Buffer.Slice(consumed, remaining).CopyTo(actor.Buffer);
            }

            if (actor.Socket.Connected)
                actor.Disconnect();
            // Disconnect the client
            Disconnecting(actor);
        }

        /// <summary>
        ///     Splitting splits the actor's receive buffer into multiple packets that can
        ///     then be processed by Received individually. The default behavior of this method
        ///     unless otherwise overridden is to split packets from the buffer using an unsigned
        ///     short packet header for the length of each packet.
        /// </summary>
        /// <param name="buffer">Actor for consuming bytes from the buffer</param>
        /// <param name="examined">Number of examined bytes from the receive</param>
        /// <param name="consumed">Number of consumed bytes by the split reader</param>
        /// <returns>Returns true if the client should remain connected.</returns>
        protected virtual bool Splitting(TActor actor, int examined, ref int consumed)
        {
            // Consume packets from the socket buffer
            var buffer = actor.Buffer.Span;
            while (consumed + 2 < examined)
            {
                var length = BitConverter.ToUInt16(buffer.Slice(consumed, 2));
                var expected = consumed + length + FooterLength;
                if (length > buffer.Length) return false;
                if (expected > examined) break;

                Received(actor, buffer.Slice(consumed, length + FooterLength));
                consumed += length + FooterLength;
            }

            return true;
        }

        /// <summary>
        ///     Disconnecting is called when the client is disconnecting from the server. Allows
        ///     the server to handle client events post-disconnect, and reclaim resources first
        ///     leased to the client on accept.
        /// </summary>
        /// <param name="actor">Actor being disconnected</param>
        private void Disconnecting(TActor actor)
        {
            // Reclaim resources and release back to server pools
            actor.Buffer.Span.Clear();
            BufferPool.Push(actor.Buffer);
            AcceptanceSemaphore.Release();
            Registry.RemoveActiveClient(actor.IPAddress);

            // Complete processing for disconnect
            Disconnected(actor);
        }

        public Task CloseAsync()
        {
            ShutdownToken.Cancel();
            return Task.CompletedTask;
        }
    }
}
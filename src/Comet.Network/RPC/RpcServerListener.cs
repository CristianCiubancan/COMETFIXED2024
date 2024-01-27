// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - RpcServerListener.cs
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

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

#endregion

namespace Comet.Network.RPC
{
    /// <summary>
    ///     Creates a TCP listener to listen for multiple remote procedure call requests on.
    ///     RPC is implemented as JSON-RPC over TCP (the web socket / network stream). Once
    ///     connected, clients will continue operating and sending requests for the life of
    ///     the TCP connection or until the client is closed. The RPC network stream should
    ///     never be exposed to the naked internet (use security groups or virtual networks
    ///     if splitting between two VMs).
    /// </summary>
    public class RpcServerListener
    {
        // Fields and Properties
        protected TcpListener BaseListener;
        protected CancellationTokenSource ShutdownToken;
        private readonly IRpcServerTarget Target;

        /// <summary>
        ///     Instantiates a new instance of <see cref="RpcServerListener" /> using a
        ///     target class of remote procedures.
        /// </summary>
        /// <param name="target">Target class for defining the RPC interface</param>
        public RpcServerListener(IRpcServerTarget target)
        {
            Target = target;
        }

        /// <summary>
        ///     Binds the TCP listener to a port and network interface. Specify "0.0.0.0"
        ///     as the endpoint address to bind to all interfaces on the host machine. Starts
        ///     the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public Task StartAsync(int port, string address = "0.0.0.0")
        {
            ShutdownToken = new CancellationTokenSource();
            BaseListener = new TcpListener(IPAddress.Parse(address), port);
            BaseListener.Start();
            return AcceptingAsync();
        }

        /// <summary>
        ///     Accepting accepts client connections asynchronously as a new task. As a client
        ///     connection is accepted, it will be associated with a new JSON-RPC wrapper. The
        ///     server will start processing requests from the client immediately after.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task AcceptingAsync()
        {
            while (BaseListener.Server.IsBound && !ShutdownToken.IsCancellationRequested)
            {
                var socket = await BaseListener.AcceptSocketAsync();
                var task = Task.Run(() => ReceivingAsync(socket));
            }
        }

        /// <summary>
        ///     Receives commands from the RPC client connection.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ReceivingAsync(Socket socket)
        {
            await using var stream = new NetworkStream(socket, true);
            // Initialize streams
            Stream input = new BufferedStream(stream);
            Stream output = new BufferedStream(stream);

            // Attach JSON-RPC wrapper
            var rpc = JsonRpc.Attach(output, input, Target);
            await rpc.Completion;
        }
    }
}
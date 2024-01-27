// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - TcpServerActor.cs
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Comet.Network.Packets;
using Comet.Network.Security;
using Comet.Shared;
using Microsoft.Extensions.Logging;
using LogLevel = Comet.Shared.LogLevel;

#endregion

namespace Comet.Network.Sockets
{
    /// <summary>
    ///     Actors are assigned to accepted client sockets to give connected clients a state
    ///     across socket operations. This allows the server to handle multiple receive writes
    ///     across single processing reads, and keep a buffer alive for faster operations.
    /// </summary>
    public abstract class TcpServerActor
    {
        // Fields and Properties
        public readonly Memory<byte> Buffer;
        public readonly ICipher Cipher;
        public readonly Socket Socket;
        public readonly uint Partition;
        private readonly object SendLock;
        public readonly byte[] PacketFooter;

        /// <summary>
        ///     Instantiates a new instance of <see cref="TcpServerActor" /> using an accepted
        ///     client socket and preallocated buffer from the server listener.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <param name="buffer">Preallocated buffer for socket receive operations</param>
        /// <param name="cipher">Cipher for handling client encipher operations</param>
        /// <param name="partition">Packet processing partition, default is disabled</param>
        public TcpServerActor(
            Socket socket,
            Memory<byte> buffer,
            ICipher cipher,
            uint partition = 0,
            string packetFooter = "")
        {
            Buffer = buffer;
            Cipher = cipher;
            Socket = socket;
            PacketFooter = Encoding.ASCII.GetBytes(packetFooter);
            Partition = partition;
            SendLock = new object();

            IPAddress = (Socket.RemoteEndPoint as IPEndPoint)?.Address.MapToIPv4().ToString();
        }

        public bool Exchanged = false;

        /// <summary>
        ///     Returns the remote IP address of the connected client.
        /// </summary>
        public string IPAddress { get; }

        /// <summary>
        ///     Sends a packet to the game client after encrypting bytes. This may be called
        ///     as-is, or overridden to provide channel functionality and thread-safety around
        ///     the accepted client socket. By default, this method locks around encryption
        ///     and sending data.
        /// </summary>
        /// <param name="packet">Bytes to be encrypted and sent to the client</param>
        public virtual Task<int> SendAsync(byte[] packet)
        {
            var encrypted = new byte[packet.Length + this.PacketFooter.Length];
            packet.CopyTo(encrypted, 0);

            BitConverter.TryWriteBytes(encrypted, (ushort)packet.Length);
            Array.Copy(this.PacketFooter, 0, encrypted, packet.Length, this.PacketFooter.Length);

            lock (SendLock)
            {
                try
                {
                    if (Socket?.Connected != true)
                        return Task.FromResult(-1);

                    Cipher?.Encrypt(encrypted, encrypted);
                    return Socket.SendAsync(encrypted, SocketFlags.None);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode < SocketError.ConnectionAborted ||
                        e.SocketErrorCode > SocketError.Shutdown)
                        Console.WriteLine(e);
                    return Task.FromResult(-1);
                }
                catch (Exception ex)
                {
                    Log.WriteLogAsync("TcpServerActor-SendAsync", LogLevel.Exception, ex.ToString()).ConfigureAwait(false);
                    return Task.FromResult(-1);
                }
            }
        }

        /// <summary>
        ///     Sends a packet to the game client after encrypting bytes. This may be called
        ///     as-is, or overridden to provide channel functionality and thread-safety around
        ///     the accepted client socket. By default, this method locks around encryption
        ///     and sending data.
        /// </summary>
        /// <param name="packet">Packet to be encrypted and sent to the client</param>
        public virtual Task<int> SendAsync(IPacket packet)
        {
            return SendAsync(packet.Encode());
        }

        /// <summary>
        ///     Force closes the client connection.
        /// </summary>
        public virtual void Disconnect()
        {
            Socket?.Disconnect(false);
        }
    }
}
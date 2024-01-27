// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Client.cs
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
using System.Net.Sockets;
using Comet.Account.Database.Models;
using Comet.Network.Security;
using Comet.Network.Sockets;

#endregion

namespace Comet.Account.States
{
    /// <summary>
    ///     Client encapsules the accepted client socket's actor and account server state.
    ///     The class should be initialized by the server's Accepted method and returned
    ///     to be passed along to the Receive loop and kept alive.
    /// </summary>
    public sealed class Client : TcpServerActor
    {
        // Fields and Properties
        public DbAccount Account;

        /// <summary>
        ///     Instantiates a new instance of <see cref="Client" /> using the Accepted event's
        ///     resulting socket and preallocated buffer. Initializes all account server
        ///     states, such as the cipher used to decrypt and encrypt data.
        /// </summary>
        /// <param name="socket">Accepted remote client socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <param name="partition">Packet processing partition</param>
        public Client(Socket socket, Memory<byte> buffer, uint partition)
            : base(socket, buffer, new TQCipher(), partition)
        {
            Exchanged = true;
        }

        public DbRealm Realm { get; set; }
        public uint Seed { get; set; }
    }
}
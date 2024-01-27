// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - MsgConnect.cs
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
using System.Threading.Tasks;
using Comet.Account.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Account.Packets
{
    // <remarks>Packet Type 1052</remarks>
    /// <summary>
    ///     Message containing a connection request to the game server. Contains the player's
    ///     access token from the Account server, and the patch and language versions of the
    ///     game client.
    /// </summary>
    public sealed class MsgConnect : MsgBase<Client>
    {
        // Static properties from server initialization
        public static bool StrictAuthentication { get; set; }

        // Packet Properties
        public ulong Token { get; set; }
        public ushort Patch { get; set; }
        public string Language { get; set; }
        public int Version { get; set; }

        /// <summary>
        ///     Decodes a byte packet into the packet structure defined by this message class.
        ///     Should be invoked to structure data from the client for processing. Decoding
        ///     follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Token = reader.ReadUInt64();
            Patch = reader.ReadUInt16();
            Language = reader.ReadString(10);
            Version = Convert.ToInt32(reader.ReadInt32().ToString(), 2);
        }

        public override Task ProcessAsync(Client client)
        {
            byte a = (byte) (Patch >> 8);
            byte b = (byte) Patch;

            return base.ProcessAsync(client);
        }
    }
}
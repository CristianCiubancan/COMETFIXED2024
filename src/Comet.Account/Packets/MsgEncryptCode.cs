// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - MsgEncryptCode.cs
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

using Comet.Account.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Account.Packets
{
    /// <remarks>Packet Type 1059</remarks>
    /// <summary>
    ///     Message sent to the game client on connect containing a random seed for generating
    ///     keys in the RC5 password cipher. This message is only used in patches after and
    ///     relative to 5174.
    /// </summary>
    public sealed class MsgEncryptCode : MsgBase<Client>
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="MsgEncryptCode" /> on client connect.
        /// </summary>
        /// <param name="seed">Random generated seed</param>
        public MsgEncryptCode(uint seed)
        {
            Seed = seed;
        }

        // Packet Properties
        public uint Seed { get; set; }

        /// <summary>
        ///     Encodes the packet structure defined by this message class into a byte packet
        ///     that can be sent to the client. Invoked automatically by the client's send
        ///     method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort) PacketType.MsgEncryptCode);
            writer.Write(Seed);
            return writer.ToArray();
        }
    }
}
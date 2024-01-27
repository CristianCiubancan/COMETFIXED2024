// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - MsgPCNum.cs
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

using System.Threading.Tasks;
using Comet.Account.Database;
using Comet.Account.States;
using Comet.Network.Packets;
using Comet.Shared.Models;

#endregion

namespace Comet.Account.Packets
{
    public sealed class MsgPCNum : MsgBase<Client>
    {
        public uint AccountIdentity;
        public string MacAddress;

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Type = (PacketType) reader.ReadUInt16();
            AccountIdentity = reader.ReadUInt32();
            MacAddress = reader.ReadString(12);
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgPCNum);
            writer.Write(AccountIdentity);
            writer.Write(MacAddress, 12);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            if (client.Account.AccountID != AccountIdentity)
                return;

            client.Account.MacAddress = MacAddress;
            await BaseRepository.SaveAsync(client.Account);

            await client.Realm.Server.SendAsync(this);
        }
    }
}
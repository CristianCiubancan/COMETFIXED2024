// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - DbRealm.cs
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
using System.ComponentModel.DataAnnotations.Schema;
using Comet.Account.States;
using Comet.Network.RPC;

#endregion

namespace Comet.Account.Database.Models
{
    /// <summary>
    ///     Realms are configured instances of the game server. This class defines routing
    ///     details for authenticated clients to be redirected to. Redirection involves
    ///     access token leasing, provided by the game server via RPC. Security for RPC stream
    ///     encryption is defined in this class.
    /// </summary>
    [Table("realm")]
    public class DbRealm
    {
        [NotMapped] public GameServer Server;

        // Column Properties
        public uint RealmID { get; set; }
        public string Name { get; set; }
        public ushort AuthorityID { get; set; }
        public string GameIPAddress { get; set; }
        public uint GamePort { get; set; }
        public string RpcIPAddress { get; set; }
        public uint RpcPort { get; set; }
        public RealmStatus Status { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LastPing { get; set; }
        public string DatabaseHost { get; set; }
        public string DatabasePass { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabaseSchema { get; set; }

        // Navigational Properties
        public virtual DbAccountAuthority Authority { get; set; }

        public enum RealmStatus : byte
        {
            Offline,
            Online,
            Busy,
            Crowded,
            Maintenance
        }
    }
}
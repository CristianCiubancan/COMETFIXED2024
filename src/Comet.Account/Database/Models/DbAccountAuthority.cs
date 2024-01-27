// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - DbAccountAuthority.cs
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Comet.Account.Database.Models
{
    /// <summary>
    ///     Account authority level defines the permissions a player has on the server to
    ///     access restricted areas, run game server administrative commands, etc. Linked
    ///     from the account table using a foreign key.
    /// </summary>
    [Table("account_authority")]
    public class DbAccountAuthority
    {
        /// <summary>
        ///     Initializes navigational properties for <see cref="DbAccountAuthority" />.
        /// </summary>
        public DbAccountAuthority()
        {
            Account = new HashSet<DbAccount>();
            Realms = new HashSet<DbRealm>();
        }

        // Column Properties
        public ushort AuthorityID { get; set; }
        public string AuthorityName { get; set; }

        // Navigational Properties
        public virtual ICollection<DbAccount> Account { get; set; }
        public virtual ICollection<DbRealm> Realms { get; set; }
    }
}
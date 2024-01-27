// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - DbAccountStatus.cs
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
    ///     Account status identifier for account standings, which define how the server permits
    ///     access to an account. For example, an account may be locked in cases where the
    ///     player's password was entered but secondary authentication failed. An account may
    ///     also be banned from the server, and not permitted access.
    /// </summary>
    [Table("account_status")]
    public class DbAccountStatus
    {
        /// <summary>
        ///     Initializes navigational properties for <see cref="DbAccountAuthority" />.
        /// </summary>
        public DbAccountStatus()
        {
            Account = new HashSet<DbAccount>();
        }

        // Column Properties
        public ushort StatusID { get; set; }
        public string StatusName { get; set; }

        // Navigational Properties
        public virtual ICollection<DbAccount> Account { get; set; }
    }
}
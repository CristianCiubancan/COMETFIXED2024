// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - DbAccount.cs
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Comet.Account.Database.Models
{
    /// <summary>
    ///     Account information for a registered player. The account server uses this information
    ///     to authenticate the player on login, and track permissions and player access to the
    ///     server. Passwords are hashed using a salted SHA-1 for user protection.
    /// </summary>
    [Table("account")]
    public class DbAccount
    {
        /// <summary>
        ///     Initializes navigational properties for <see cref="DbAccount" />.
        /// </summary>
        public DbAccount()
        {
            Logins = new HashSet<DbLogin>();
        }

        // Column Properties
        public uint AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public ushort AuthorityID { get; set; }
        public ushort StatusID { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public DateTime Registered { get; set; }
        public byte VipLevel { get; set; }

        // Navigational Properties
        public virtual DbAccountAuthority Authority { get; set; }
        public virtual DbAccountStatus Status { get; set; }
        public virtual ICollection<DbLogin> Logins { get; set; }
    }
}
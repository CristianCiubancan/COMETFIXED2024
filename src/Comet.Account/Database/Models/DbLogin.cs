// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - DbLogin.cs
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

#endregion

namespace Comet.Account.Database.Models
{
    /// <summary>
    ///     Record of a successful login to the account server. These records can be used to
    ///     debug client connectivity issues, or outline connection patterns used to identify
    ///     wrongful logins.
    /// </summary>
    [Table("logins")]
    public class DbLogin
    {
        // Column Properties
        public DateTime Timestamp { get; set; }
        public uint AccountID { get; set; }
        public string IPAddress { get; set; }

        // Navigational Properties
        public virtual DbAccount Account { get; set; }
    }
}
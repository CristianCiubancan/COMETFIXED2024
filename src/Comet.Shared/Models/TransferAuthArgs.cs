// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Shared - TransferAuthArgs.cs
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

namespace Comet.Shared.Models
{
    /// <summary>
    ///     Defines account parameters to be transferred from the account server to the game
    ///     server. Account information is supplied from the account database, and used on
    ///     the game server to transfer authentication and authority level.
    /// </summary>
    public class TransferAuthArgs
    {
        public string IPAddress { get; set; }
        public uint AccountID { get; set; }
        public ushort AuthorityID { get; set; }
        public string AuthorityName { get; set; }
        public byte VipLevel { get; set; }
    }
}
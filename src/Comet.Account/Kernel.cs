// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Kernel.cs
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comet.Account.Database.Models;
using Comet.Account.States;
using Comet.Network.Services;

#endregion

namespace Comet.Account
{
    /// <summary>
    ///     Kernel for the server, acting as a central core for pools of models and states
    ///     initialized by the server. Used in database repositories to load data into memory
    ///     from essential tables or tables which require heavy post-processing. Used in the
    ///     server packet process methods for tracking client and world states.
    /// </summary>
    public static class Kernel
    {
        public static Dictionary<string, DbRealm> Realms;
        public static ConcurrentDictionary<uint, Client> Clients = new ConcurrentDictionary<uint, Client>();
        public static ConcurrentDictionary<uint, Player> Players = new ConcurrentDictionary<uint, Player>();

        // Background services
        public static class Services
        {
            public static RandomnessService Randomness = new RandomnessService();
        }

        /// <summary>
        /// Returns the next random number from the generator.
        /// </summary>
        /// <param name="minValue">The least legal value for the Random number.</param>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public static Task<int> NextAsync(int minValue, int maxValue) =>
            Services.Randomness.NextAsync(minValue, maxValue);
    }
}
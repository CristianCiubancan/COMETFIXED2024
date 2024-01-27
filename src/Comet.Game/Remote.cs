// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Remote.cs
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
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Security.Cryptography;
using Comet.Network.RPC;
using Comet.Shared;
using Comet.Shared.Models;
using Newtonsoft.Json;

#endregion

namespace Comet.Game
{
    /// <summary>
    ///     Remote procedures that can be called from the account server to perform a game
    ///     server action. Methods in this class are automatically registered with the RPC
    ///     server on server load.
    /// </summary>
    public class Remote : IRpcServerTarget
    {
        public Socket Socket { get; private set; }
        public string AgentName { get; private set; }


        /// <summary>
        ///     Called to ensure that the connection to the RPC server has been successful,
        ///     and that any wire security placed on the connection has been initialized and
        ///     is working correctly.
        /// </summary>
        /// <param name="agentName">Name of the client connecting</param>
        public void Connected(string agentName)
        {
            AgentName = agentName;
            Log.WriteLogAsync(LogLevel.Info, "{0} has connected", agentName).ConfigureAwait(false);
        }

        /// <summary>
        ///     Transfers authentication information directly from the account server on
        ///     successful client login. The game server will generate a new access token,
        ///     and return the token to the account server for client redirection.
        /// </summary>
        /// <param name="args">Authentication details from the account server.</param>
        /// <returns>Returns an access token for the game server.</returns>
        public ulong TransferAuth(TransferAuthArgs args)
        {
            // Generate the access token
            var bytes = new byte[8];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = BitConverter.ToUInt64(bytes);

            // Store in the login cache with an absolute timeout
            var timeoutPolicy = new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60)};
            Kernel.Logins.Set(token.ToString(), args, timeoutPolicy);
            return token;
        }

        public void TransferMacAddress(TransferMacAddrArgs args)
        {
            Log.WriteLogAsync(LogLevel.Debug, $"TransferMacAddress data: {Environment.NewLine}{JsonConvert.SerializeObject(args)}").ConfigureAwait(false);
        }
    }
}
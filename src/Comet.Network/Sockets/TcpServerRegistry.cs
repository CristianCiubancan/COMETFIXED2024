// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - TcpServerRegistry.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

#endregion

namespace Comet.Network.Sockets
{
    /// <summary>
    ///     TcpServerRegistry gives the server basic flood protection by keeping a registry of
    ///     connection attempts, blocked connections, and active connections. A background worker
    ///     will clean blocked records automatically.
    ///     ///
    /// </summary>
    public sealed class TcpServerRegistry : IHostedService, IDisposable
    {
        private readonly int BanMinutes;
        private readonly int MaxActiveConnections;

        private readonly int MaxAttemptsPerMinute;

        // Fields and properties
        private Dictionary<string, int> Active;
        private object ActiveMutex;
        private Dictionary<string, int> Attempts;
        private object AttemptsMutex;
        private ConcurrentDictionary<string, DateTime> Blocks;
        private Timer PurgeTimer;

        /// <summary>
        ///     Instantiates a new instance of <see cref="TcpServerRegistry" /> with initialized
        ///     collections for connection registration checks. The background worker for
        ///     trimming connections does not start until Start is called.
        /// </summary>
        /// <param name="banMinutes">Minutes a ban should remain in effect for</param>
        /// <param name="maxActiveConn">Maximum active connections alive at any given time</param>
        /// <param name="maxAttemptsPerMinute">Maximum connection attempts per minute</param>
        public TcpServerRegistry(
            int banMinutes = 15,
            int maxActiveConn = 10,
            int maxAttemptsPerMinute = 15)
        {
            Active = new Dictionary<string, int>();
            Attempts = new Dictionary<string, int>();
            Blocks = new ConcurrentDictionary<string, DateTime>();

            ActiveMutex = new object();
            AttemptsMutex = new object();
            BanMinutes = banMinutes;
            MaxActiveConnections = maxActiveConn;
            MaxAttemptsPerMinute = maxAttemptsPerMinute;
        }

        /// <summary>Disposes of the purge timer.</summary>
        public void Dispose()
        {
            PurgeTimer?.Dispose();
        }

        /// <summary>
        ///     Triggered when the application host is ready to start cleaning connection records
        ///     from the registry. Blocked entries with expired times will be unblocked, and
        ///     attempt counters will be reset.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            PurgeTimer = new Timer(
                this.TimedPurgeJob,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        /// <summary>Stops cleaning attempt counters and stop checking for bans.</summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            PurgeTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Adds a new active client to the registry of connections. If the maximum number
        ///     of active connections has been exceeded for an IP address, or the accept volume
        ///     has spiked beyond permitted limits, this method will return false and ban the
        ///     client if evaluated to be an attack.
        /// </summary>
        /// <param name="ip">IPv4 address of the client</param>
        /// <returns>True if the connection is allowed.</returns>
        public bool AddActiveClient(string ip)
        {
            // Check for blocked IP addresses
            if (Blocks.ContainsKey(ip)) return false;

            // Check if the client should be blocked for frequent connections and then 
            // increment the active connections counter if the previous operation succeeded.
            return IncrementCounter(ip, MaxAttemptsPerMinute, ref AttemptsMutex, ref Attempts)
                   && IncrementCounter(ip, MaxActiveConnections, ref ActiveMutex, ref Active);
        }

        /// <summary>
        ///     Increments a counter given a collection of counts keyed by the client's IP
        ///     address. If the counter exceeds the ceiling value set by the parent method, then
        ///     the connection will be banned.
        /// </summary>
        /// <param name="ip">IPv4 address of the client</param>
        /// <param name="ceiling">Highest counter value before banning the connection</param>
        /// <param name="mutex">Mutex for locking the counter collection</param>
        /// <param name="collection">Counter collection keyed by IP address</param>
        /// <returns>True if the counter was incremented and the client wasn't banned.</returns>
        public bool IncrementCounter(
            string ip,
            int ceiling,
            ref object mutex,
            ref Dictionary<string, int> collection)
        {
            lock (mutex)
            {
                if (collection.TryGetValue(ip, out int count))
                {
                    count++;
                    if (count > MaxActiveConnections)
                    {
                        Blocks.TryAdd(ip, DateTime.Now.AddMinutes(BanMinutes));
                        return false;
                    }

                    collection[ip] = count;
                }
                else
                {
                    collection.TryAdd(ip, 1);
                }
            }

            return true;
        }

        /// <summary>Removes an active connection from the registry.</summary>
        /// <param name="ip">IPv4 address of the client</param>
        public void RemoveActiveClient(string ip)
        {
            // Decrement active connections count
            lock (ActiveMutex)
            {
                if (Active.TryGetValue(ip, out int attempts))
                {
                    attempts--;
                    if (attempts == 0)
                        Active.Remove(ip);
                    else
                        Active[ip] = attempts;
                }
            }
        }

        /// <summary>
        ///     Invoked on a timer to purge attempt counters and check for expired bans. If the
        ///     interval calling this method isn't fast enough, it could cause players to get
        ///     banned unjustly.
        /// </summary>
        public void TimedPurgeJob(object state)
        {
            lock (AttemptsMutex)
            {
                Attempts.Clear();
            }

            DateTime now = DateTime.Now;
            foreach (var blockedConnection in Blocks)
                if (blockedConnection.Value < now)
                    Blocks.TryRemove(blockedConnection.Key, out DateTime _);
        }
    }
}
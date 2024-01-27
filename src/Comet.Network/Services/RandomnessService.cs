// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - RandomnessService.cs
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
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

#endregion

namespace Comet.Network.Services
{
    /// <summary>
    /// This background service instantiates a single Random instance to generate all random
    /// numbers for the server. This allows the server to generate random numbers across 
    /// multiple threads without generating the same number or returning zero. This service
    /// in particular buffers random numbers to a channel to avoid locking.
    /// </summary>
    public class RandomnessService : BackgroundService
    {
        // Fields and Properties
        private readonly Channel<double> BufferChannel;

        /// <summary>
        /// Instantiates a new instance of <see cref="RandomnessService"/> using a default
        /// capacity to buffer random numbers.
        /// </summary>
        /// <param name="capacity">Capacity of the bounded channel.</param>
        public RandomnessService(int capacity = 10000)
        {
            BufferChannel = Channel.CreateBounded<double>(capacity);
        }

        /// <summary>
        /// Triggered when the application host is ready to start queuing random numbers.
        /// Since the channel holding random numbers is bounded, writes will block 
        /// naturally on an await rather than locking threads to generate numbers.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                await BufferChannel.Writer.WriteAsync(
                    RandomNumberGenerator.GetInt32(int.MaxValue) / (double)int.MaxValue,
                    stoppingToken);
        }

        /// <summary>Returns the next random number from the generator.</summary>
        /// <param name="minValue">The least legal value for the Random number.</param>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public async Task<int> NextAsync(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var range = (long) maxValue - minValue;
            if (range > int.MaxValue)
                throw new ArgumentOutOfRangeException();

            var value = await BufferChannel.Reader.ReadAsync();
            var result = (int) (value * range) + minValue;
            return result;
        }

        /// <summary>Writes random numbers from the generator to a buffer.</summary>
        /// <param name="buffer">Buffer to write bytes to.</param>
        public async Task NextBytesAsync(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte) await NextAsync(0, 255);
        }
    }
}
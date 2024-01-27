// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PrimeGeneratorService.cs
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
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Math;

#endregion

namespace Comet.Network.Services
{
    /// <summary>
    /// This background service uses a single Random instance to generate all probable 
    /// primes for the server. This is used in the Diffie Hellman Key Exchange. 
    /// </summary>
    public class PrimeGeneratorService : BackgroundService
    {
        // Fields and Properties
        private int BitLength;
        private Channel<BigInteger> BufferChannel;
        protected Random Generator;

        /// <summary>
        /// Instantiates a new instance of <see cref="PrimeGeneratorService"/> using a
        /// default capacity to buffer probable primes.
        /// </summary>
        /// <param name="capacity">Capacity of the bounded channel.</param>
        /// <param name="bitLength">Bit length of probable primes generated.</param>
        public PrimeGeneratorService(int capacity = 100, int bitLength = 256)
        {
            BitLength = bitLength;
            BufferChannel = Channel.CreateBounded<BigInteger>(capacity);
            Generator = new Random(); //NOSONAR
        }

        /// <summary>
        /// Triggered when the application host is ready to start queuing probable primes.
        /// Since the channel holding probable primes is bounded, writes will block 
        /// naturally on an await rather than locking threads to generate probable primes.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                await BufferChannel.Writer.WriteAsync(
                    BigInteger.ProbablePrime(BitLength, Generator),
                    stoppingToken);
        }

        /// <summary>Returns the next probable prime from the generator.</summary>
        public Task<BigInteger> NextAsync()
        {
            return BufferChannel.Reader.ReadAsync().AsTask();
        }
    }
}
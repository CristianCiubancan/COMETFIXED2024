// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - DiffieHellman.cs
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

using System.Threading.Tasks;
using Comet.Network.Services;
using Org.BouncyCastle.Math;

#endregion

namespace Comet.Network.Security
{
    /// <summary>
    /// This implementation of the Diffie Hellman Key Exchange implements the base modulo
    /// and big number operations without a hash algorithm. This is non-standard, and was
    /// later fixed in higher versions of Conquer Online using MD5.
    /// </summary>
    public sealed class DiffieHellman
    {
        private const string DefaultGenerator = "05";
        private const string DefaultPrimativeRoot = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";

        // Constants and static properties
        public static readonly PrimeGeneratorService ProbablePrimes;

        /// <summary>
        /// Generate the modulus integer as a static constant. This is an unfortunate
        /// consequence of creating a randomness service for generating numbers in a
        /// thread safe environment, and using a language with poor multithreading 
        /// support.
        /// </summary>
        static DiffieHellman()
        {
            ProbablePrimes = new PrimeGeneratorService();
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="DiffieHellman"/> key exchange.
        /// If no prime root or generator is specified, then defaults for remaining W
        /// interoperable with the Conquer Online game client will be used. 
        /// </summary>
        /// <param name="p">Prime root to modulo with the generated probable prime.</param>
        /// <param name="g">Generator used to seed the modulo of primes.</param>
        public DiffieHellman(
            string p = DefaultPrimativeRoot,
            string g = DefaultGenerator)
        {
            PrimeRoot = new BigInteger(p, 16);
            Generator = new BigInteger(g, 16);
            DecryptionIV = new byte[8];
            EncryptionIV = new byte[8];
        }

        // Key exchange Properties
        public BigInteger PrimeRoot { get; set; }
        public BigInteger Generator { get; set; }
        public BigInteger Modulus { get; set; }
        public BigInteger PublicKey { get; private set; }
        public BigInteger PrivateKey { get; private set; }

        // Blowfish IV exchange properties
        public byte[] DecryptionIV { get; private set; }
        public byte[] EncryptionIV { get; private set; }

        /// <summary>Computes the public key for sending to the client.</summary>
        public async Task ComputePublicKeyAsync()
        {
            if (Modulus == null)
                Modulus = await ProbablePrimes.NextAsync();
            PublicKey = Generator.ModPow(Modulus, PrimeRoot);
        }

        /// <summary>Computes the private key given the client response.</summary>
        /// <param name="clientKeyString">Client key from the exchange</param>
        /// <returns>Bytes representing the private key for Blowfish Cipher.</returns>
        public void ComputePrivateKey(string clientKeyString)
        {
            BigInteger clientKey = new BigInteger(clientKeyString, 16);
            PrivateKey = clientKey.ModPow(Modulus, PrimeRoot);
        }
    }
}
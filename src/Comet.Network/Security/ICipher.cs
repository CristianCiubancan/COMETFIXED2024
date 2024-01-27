// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - ICipher.cs
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
using Comet.Network.Sockets;

#endregion

namespace Comet.Network.Security
{
    using System;

    /// <summary>
    /// Defines generalized methods for ciphers used by 
    /// <see cref="TcpServerActor"/> and 
    /// <see cref="TcpServerListener{TActor}"/> for encrypting and decrypting
    /// data to and from the game client. Can be used to switch between ciphers easily for
    /// seperate states of the game client connection.
    /// </summary>
    public interface ICipher
    {
        /// <summary>Generates keys using key derivation variables.</summary>
        /// <param name="seeds">Initialized seeds for generating keys</param>
        void GenerateKeys(object[] seeds);

        /// <summary>Decrypts data from the client</summary>
        /// <param name="src">Source span that requires decrypting</param>
        /// <param name="dst">Destination span to contain the decrypted result</param>
        void Decrypt(Span<byte> src, Span<byte> dst);

        /// <summary>Encrypts data to send to the client</summary>
        /// <param name="src">Source span that requires encrypting</param>
        /// <param name="dst">Destination span to contain the encrypted result</param>
        void Encrypt(Span<byte> src, Span<byte> dst);
    }

}
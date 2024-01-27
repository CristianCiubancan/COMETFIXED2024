// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PacketReader.cs
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
using System.IO;
using System.Text;

#endregion

namespace Comet.Network.Packets
{
    /// <summary>
    ///     Reader that implements methods for reading bytes from a binary stream reader,
    ///     used to help decode packet structures using TQ Digital's byte ordering rules.
    ///     String processing has been overloaded for supporting TQ's byte-length prefixed
    ///     strings and fixed strings.
    /// </summary>
    public sealed class PacketReader : BinaryReader, IDisposable
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="PacketReader" /> using a supplied
        ///     array of packet bytes. Creates a new binary reader for the derived class
        ///     to read from.
        /// </summary>
        /// <param name="bytes">Packet bytes to be read in</param>
        public PacketReader(byte[] bytes) : base(new MemoryStream(bytes), CodePagesEncodingProvider.Instance.GetEncoding(1252) ?? Encoding.ASCII)
        {
        }

        /// <summary>
        ///     Reads a string from the current stream. The string is prefixed with the byte
        ///     length and encoded as an ASCII string. <see cref="EndOfStreamException" /> is
        ///     thrown if the full string cannot be read from the binary reader.
        /// </summary>
        /// <returns>Returns the resulting string from the read.</returns>
        public override string ReadString()
        {
            return base.ReadString().TrimEnd('\0');
        }

        /// <summary>
        ///     Reads a string from the current stream. The string is fixed with a known
        ///     string length before reading from the stream and encoded as an ASCII string.
        ///     <see cref="EndOfStreamException" /> is thrown if the full string cannot be
        ///     read from the binary reader.
        /// </summary>
        /// <param name="fixedLength">Length of the string to be read</param>
        /// <returns>Returns the resulting string from the read.</returns>
        public string ReadString(int fixedLength)
        {
            return Encoding.ASCII.GetString(ReadBytes(fixedLength)).TrimEnd('\0');
        }

        /// <summary>
        ///     Reads a list of strings from the current stream. The string list is prefixed
        ///     with the byte amount of strings in the list. Then, each string in the list is
        ///     prefixed with the length of that string and encoded as an ASCII string.
        ///     <see cref="EndOfStreamException" /> is thrown if the full string cannot be read
        ///     from the binary reader.
        /// </summary>
        /// <returns>Returns the resulting list of strings from the read.</returns>
        public List<string> ReadStrings()
        {
            var strings = new List<string>();
            var amount = ReadByte();
            for (int i = 0; i < amount; i++)
                strings.Add(ReadString());
            return strings;
        }

        #region IDisposable Support

        private bool DisposedValue; // To detect redundant calls

        /// <summary>
        ///     Called from the Dispose method to dispose of class resources once and only
        ///     once using the Disposable design pattern. Calls into the base dispose method
        ///     after disposing of class resources first.
        /// </summary>
        /// <param name="disposing">True if clearing unmanaged and managed resources</param>
        private new void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    BaseStream.Close();
                    BaseStream.Dispose();
                }

                base.Dispose(disposing);
                DisposedValue = true;
            }
        }

        /// <summary>
        ///     Called to dispose the class.
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
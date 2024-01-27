// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PacketWriter.cs
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
    ///     Writer that implements methods for writing bytes to a binary stream writer, used
    ///     to help encode packet structures using TQ Digital's byte ordering rules. Packets
    ///     sent using the <see cref="TcpServerActor" />'s send method do not need to be
    ///     written with a prefixed length. The send method will include this with the final
    ///     result of the packet writer.
    /// </summary>
    public sealed class PacketWriter : BinaryWriter, IDisposable
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="PacketWriter" /> and writes the
        ///     first unsigned short to the stream as a placeholder for the packet length.
        /// </summary>
        public PacketWriter() : base(new MemoryStream(), CodePagesEncodingProvider.Instance.GetEncoding(1252) ?? Encoding.ASCII)
        {
            base.Write((ushort) 0);
        }

        /// <summary>
        ///     Writes a string to the current stream. The string is prefixed with the byte
        ///     length and encoded as an ASCII string.
        /// </summary>
        /// <param name="value">String value to be written to the stream</param>
        public override void Write(string value)
        {
            base.Write((byte) value.Length);
            base.Write(value);
        }

        /// <summary>
        ///     Writes a string to the current stream. The string is fixed with a known
        ///     string length before writing the string value encoded as an ASCII string
        ///     to the stream.
        /// </summary>
        /// <param name="value">String value to be written to the stream</param>
        /// <param name="fixedLength">Length of the string to be read</param>
        public void Write(string value, int fixedLength)
        {
            var array = new byte[fixedLength];
            (CodePagesEncodingProvider.Instance.GetEncoding(1252) ?? Encoding.ASCII).GetBytes(value).CopyTo(array, 0);
            base.Write(array);
        }

        /// <summary>
        ///     Writes a list of strings to the current stream. The string list is prefixed with
        ///     the amount of strings in the list. Then, each string in the list is prefixed
        ///     with the length of that string and encoded as an ASCII string.
        /// </summary>
        /// <param name="strings">List of strings to be written to the stream</param>
        public void Write(List<string> strings)
        {
            Write((byte) strings.Count);
            for (int i = 0; i < strings.Count; i++)
                base.Write(strings[i]);
        }

        /// <summary>
        ///     Writes the packet writer's stream to a byte array, regardless of the position
        ///     of the stream. Flushes all writes to the stream before returning an array.
        /// </summary>
        /// <returns>Returns the byte array representation of the memory stream.</returns>
        public byte[] ToArray()
        {
            BaseStream.Flush();
            return (BaseStream as MemoryStream).ToArray();
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
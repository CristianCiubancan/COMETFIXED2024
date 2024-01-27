// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PacketDump.cs
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
using System.Text;

#endregion

namespace Comet.Network.Packets
{
    /// <summary>
    ///     Dumps packet bytes in a human-readable format. Primarily used to debug server
    ///     errors and missing packet structures, or to reverse engineer unknown packet
    ///     structures.
    /// </summary>
    public static class PacketDump
    {
        /// <summary>
        ///     Converts packet bytes to a hexadecimal string. The format of the hex dump
        ///     matches the output of hexdump -C from Linux command line.
        /// </summary>
        /// <param name="data">Packet data to be formatted</param>
        /// <returns>Returns the hexadecimal string created by Hex.</returns>
        public static string Hex(ReadOnlySpan<byte> data)
        {
            var text = new StringBuilder();
            for (int l = 0; l < data.Length; l += 16)
            {
                // Write the address and body
                text.AppendFormat("{0:X4}:", l);
                for (int i = l; i < l + 16; i++)
                {
                    text.Append(i % 8 == 0 ? "  " : " ");
                    text.Append(i >= data.Length ? "  " : string.Format("{0:X2}", data[i]));
                }

                // Write the ASCII conversion
                int v = l + 16 >= data.Length ? data.Length : l + 16;
                text.Append("  | ");
                for (int i = l; i < v; i++)
                    text.Append(data[i] < 32 || data[i] > 126 ? '.' : (char) data[i]);
                text.Append(" |\n");
            }

            return text.ToString();
        }
    }
}
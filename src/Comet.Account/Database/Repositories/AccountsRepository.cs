// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - AccountsRepository.cs
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Comet.Account.Database.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.Encoders;

#endregion

namespace Comet.Account.Database.Repositories
{
    /// <summary>
    ///     Repository for defining data access layer (DAL) logic for the account table. Allows
    ///     the server to fetch account details for player authentication. Accounts are fetched
    ///     on demand for each player authentication request.
    /// </summary>
    public static class AccountsRepository
    {
        /// <summary>
        ///     Fetches an account record from the database using the player's username as a
        ///     unique key for selecting a single record.
        /// </summary>
        /// <param name="username">Username to pull account info for</param>
        /// <returns>Returns account details from the database.</returns>
        public static async Task<DbAccount> FindAsync(string username)
        {
            await using var db = new ServerDbContext();
            return await db.Accounts.Include(x => x.Authority)
                .Include(x => x.Status)
                .Where(x => x.Username == username)
                .SingleOrDefaultAsync();
        }

        public static async Task<DbAccount> FindAsync(uint identity)
        {
            await using var db = new ServerDbContext();
            return await db.Accounts.Include(x => x.Authority)
                .Include(x => x.Status)
                .Where(x => x.AccountID == identity)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        ///     Validates the user's inputted password, which has been decrypted from the client
        ///     request decode method, and is ready to be hashed and compared with the SHA-1
        ///     hash in the database.
        /// </summary>
        /// <param name="input">Inputted password from the client's request</param>
        /// <param name="hash">Hashed password in the database</param>
        /// <param name="salt">Salt for the hashed password in the databse</param>
        /// <returns>Returns true if the password is correct.</returns>
        public static bool CheckPassword(string input, string hash, string salt)
        {
            return HashPassword(input, salt).Equals(hash);
        }

        public static string HashPassword(string password, string salt)
        {
            byte[] inputHashed;
            using (var sha256 = SHA256.Create())
                inputHashed = sha256.ComputeHash(Encoding.ASCII.GetBytes(password + salt));
            var final = Hex.ToHexString(inputHashed);
            return final;
        }

        public static string GenerateSalt()
        {
            const string UPPER_S = "QWERTYUIOPASDFGHJKLZXCVBNM";
            const string LOWER_S = "qwertyuioplkjhgfdsazxcvbnm";
            const string NUMBER_S = "1236547890";
            const string POOL_S = UPPER_S + LOWER_S + NUMBER_S;
            const int SIZE_I = 30;

            string output = "";
            for (int i = 0; i < SIZE_I; i++)
            {
                output += POOL_S[RandomNumberGenerator.GetInt32(int.MaxValue) % POOL_S.Length];
            }

            return output;
        }
    }
}
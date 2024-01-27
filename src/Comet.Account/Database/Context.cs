// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Context.cs
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

using Comet.Account.Database.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Comet.Account.Database
{
    /// <summary>
    ///     Server database client context implemented using Entity Framework Core, an open
    ///     source object-relational mapping framework for ADO.NET. Substitutes in MySQL
    ///     support through a third-party framework provided by Pomelo Foundation.
    /// </summary>
    public class ServerDbContext : DbContext
    {
        // Connection String Configuration
        public static ServerConfiguration.DatabaseConfiguration Configuration;

        public static void Initialize()
        {
            ConnectionString = $"server={Configuration.Hostname};database={Configuration.Schema};user={Configuration.Username};password={Configuration.Password};port={Configuration.Port}";
            ServerVersion = ServerVersion.AutoDetect(ConnectionString);
        }

        // Table Definitions
        public virtual DbSet<DbAccount> Accounts { get; set; }
        public virtual DbSet<DbAccountAuthority> AccountAuthorities { get; set; }
        public virtual DbSet<DbAccountStatus> AccountStatuses { get; set; }
        public virtual DbSet<DbLogin> Logins { get; set; }
        public virtual DbSet<DbRealm> Realms { get; set; }
        public virtual DbSet<DbRecordUser> RecordUsers { get; set; }

        private static string ConnectionString { get; set; }
        private static ServerVersion ServerVersion { get; set; }

        /// <summary>
        ///     Configures the database to be used for this context. This method is called
        ///     for each instance of the context that is created. For this project, the MySQL
        ///     connector will be initialized with a connection string from the server's
        ///     configuration file.
        /// </summary>
        /// <param name="options">Builder to create the context</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies(false);
            options.UseMySql(ConnectionString, ServerVersion, null);
        }

        /// <summary>
        ///     Typically called only once when the first instance of the context is created.
        ///     Allows for model building before the context is fully initialized, and used
        ///     to initialize composite keys and relationships.
        /// </summary>
        /// <param name="builder">Builder for creating models in the context</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbAccount>(e => e.HasKey(x => x.AccountID));
            builder.Entity<DbAccountAuthority>(e => e.HasKey(x => x.AuthorityID));
            builder.Entity<DbAccountStatus>(e => e.HasKey(x => x.StatusID));
            builder.Entity<DbLogin>(e => e.HasKey(x => new {x.AccountID, x.Timestamp}));
            builder.Entity<DbRealm>(e => e.HasKey(x => x.RealmID));
            builder.Entity<DbRealm>().Property(x => x.Status).HasConversion<byte>();
            builder.Entity<DbRealmStatus>().Property(x => x.NewStatus).HasConversion<byte>();
            builder.Entity<DbRealmStatus>().Property(x => x.OldStatus).HasConversion<byte>();
        }

        /// <summary>
        ///     Tests that the database connection is alive and configured.
        /// </summary>
        public static bool Ping()
        {
            try
            {
                using (ServerDbContext ctx = new ServerDbContext())
                    return ctx.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }
    }
}
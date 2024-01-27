using Comet.Web.App.Data.Entities;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Comet.Web.App.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken
        >, IPersistedGrantDbContext
    {
        private readonly IOptions<OperationalStoreOptions> mOperationalStoreOptions;
        private IConfiguration mConfiguration;

        public ApplicationDbContext(IConfiguration configuration, 
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            mOperationalStoreOptions = operationalStoreOptions;
            mConfiguration = configuration;
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{PersistedGrant}"/>.
        /// </summary>
        public virtual DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{DeviceFlowCodes}"/>.
        /// </summary>
        public virtual DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        #region Custom Entities
        
        public virtual DbSet<SecurityQuestion> SecurityQuestions {  get; set; }

        #endregion

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUserToken>()
                .HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            builder.Entity<ApplicationUserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<ApplicationUserLogin>()
                .HasKey(x => new { x.LoginProvider, x.ProviderKey });
            builder.Entity<DeviceFlowCodes>().HasKey(x => x.UserCode);
            builder.Entity<PersistedGrant>().HasKey(x => x.Key);

            builder.ConfigurePersistedGrantContext(mOperationalStoreOptions.Value);
        }
    }
}
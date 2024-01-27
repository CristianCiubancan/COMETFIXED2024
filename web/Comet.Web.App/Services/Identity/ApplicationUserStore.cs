using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Comet.Web.App.Services.Password;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comet.Web.App.Services.Identity
{
    public class ApplicationUserStore : UserStore<
        ApplicationUser,
        ApplicationRole,
        ApplicationDbContext,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationUserToken,
        ApplicationRoleClaim
        >
    {
        private readonly ILogger<ApplicationUserStore> mLogger;
        private readonly LanguageService mLanguageService;

        public ApplicationUserStore(ILogger<ApplicationUserStore> logger, 
            ApplicationDbContext dbContext,
            LanguageService language)
            : base(dbContext)
        {
            mLogger = logger;
            mLanguageService = language;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (await Context.Users.AnyAsync(usr => usr.NormalizedEmail.Equals(user.NormalizedEmail), cancellationToken))
                {
                    return IdentityResult.Failed(new IdentityError() { Code = "EMAIL_IN_USE", Description = mLanguageService.GetString("EmailInUse") });
                }

                if (await Context.Users.AnyAsync(usr => usr.NormalizedUserName.Equals(user.NormalizedUserName), cancellationToken))
                {
                    return IdentityResult.Failed(new IdentityError() { Code = "USERNAME_ALREADY_EXISTS", Description = mLanguageService.GetString("UserNameInUse") });
                }

                Context.Users.Update(user);
                await Context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex.ToString());
                return IdentityResult.Failed(new IdentityError() { Code = ex.HResult.ToString("X08"), Description = ex.InnerException.Message });
            }
        }
    }
}

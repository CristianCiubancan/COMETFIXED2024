using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Comet.Web.App.Services.Identity
{
    public class UserManager : UserManager<ApplicationUser>
    {
        private readonly ApplicationDbContext mDbContext;
        private IServiceProvider mServices;

        public UserManager(IUserStore<ApplicationUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<ApplicationUser> passwordHasher, 
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ApplicationDbContext dbContext,
            ILogger<UserManager<ApplicationUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            mDbContext = dbContext;
            mServices = services;
        }

        public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var result = await base.GetClaimsAsync(user);

            Dictionary<string, Claim> dictClaims = result.ToDictionary(x => x.Type);

            //// get role
            var roleUser = await mDbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            int idRole = roleUser?.RoleId ?? -1;

            // get the claims
            var roleClaims = await mDbContext.RoleClaims.Where(x => x.RoleId == idRole).ToListAsync();
            foreach (var roleClaim in roleClaims)
            {
                if (!dictClaims.ContainsKey(roleClaim.ClaimType))
                {
                    dictClaims.Add(roleClaim.ClaimType, new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
                }
                else
                {
                    dictClaims[roleClaim.ClaimType] = new Claim(roleClaim.ClaimType, roleClaim.ClaimValue);
                }
            }

            // then we get the user claims
            var userClaims = await mDbContext.UserClaims.Where(x => x.UserId == user.Id).ToListAsync();
            foreach (var userClaim in userClaims)
            {
                if (!dictClaims.ContainsKey(userClaim.ClaimType))
                {
                    dictClaims.Add(userClaim.ClaimType, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
                }
                else
                {
                    dictClaims[userClaim.ClaimType] = new Claim(userClaim.ClaimType, userClaim.ClaimValue);
                }
            }

            return dictClaims.Select(x => x.Value).ToList();
        }
    }
}

using Comet.Web.App.Areas.Profile.Models;
using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Comet.Web.App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Web.App.Areas.Profile.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> mUserManager;
        private readonly RoleManager<ApplicationRole> mRoleManager;
        private readonly ILogger<IndexModel> mLogger;
        private readonly LanguageService mLanguage;
        private readonly ApplicationDbContext mDbContext;

        public IndexModel(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<IndexModel> logger,
            LanguageService language,
            ApplicationDbContext dbContext)
        {
            mUserManager = userManager;
            mRoleManager = roleManager;
            mLogger = logger;
            mLanguage = language;
            mDbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        public ProfileModel Profile { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            bool selfProfile = false;
            ApplicationUser user;
            if (string.IsNullOrWhiteSpace(UserName)) // self-profile
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return LocalRedirect("/");
                }
                user = await mUserManager.GetUserAsync(User);
                selfProfile = true;
            }
            else
            {
                user = await mUserManager.FindByNameAsync(UserName);
            }

            if (user == null)
                return LocalRedirect("/");

            string title = mLanguage.GetString(Security.Roles.Member.ToString());

            var roleUser = await mDbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId ==  user.Id);
            if (roleUser != null)
            {
                var role = await mDbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleUser.RoleId);
                if (role != null)
                    title = mLanguage.GetString(role.Name);
            }

            Profile = new ProfileModel
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Title = title,

                LoadedByOwner = selfProfile,
                IsFriend = false
            };

            var userClaims = await mDbContext.UserClaims.Where(x => x.UserId == user.Id).ToListAsync();
            foreach (var contact in Enum.GetValues<ContactInfoType>())
            {
                var claim = userClaims.FirstOrDefault(x => x.ClaimType.Equals(contact.ToClaimString()));
                if (claim != null)
                    Profile.ContactInfo.Add(contact, claim.ClaimValue);
            }

            foreach (var personal in Enum.GetValues<PersonalInfoType>())
            {
                var claim = userClaims.FirstOrDefault(x => x.ClaimType.Equals(personal.ToClaimString()));
                if (claim != null)
                    Profile.PersonalInfo.Add(personal, claim.ClaimValue);
            }

            if (Profile.PersonalInfo.ContainsKey(PersonalInfoType.SocialName))
            {
                Profile.Name += $" ({Profile.PersonalInfo[PersonalInfoType.SocialName]})";
            }

            return Page();
        }
    }
}

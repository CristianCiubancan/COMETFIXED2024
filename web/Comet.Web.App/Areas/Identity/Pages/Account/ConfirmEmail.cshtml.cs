using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Comet.Web.App.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Comet.Web.App.Services;

namespace Comet.Web.App.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> mUserManager;
        private readonly LanguageService mLanguage;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, LanguageService language)
        {
            mUserManager = userManager;
            mLanguage = language;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await mUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(mLanguage.GetString("CouldNotFindAccountToActivate"));
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await mUserManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "AccountActivatedSuccessfully" : "CouldNotActivateAccount";
            return Page();
        }
    }
}

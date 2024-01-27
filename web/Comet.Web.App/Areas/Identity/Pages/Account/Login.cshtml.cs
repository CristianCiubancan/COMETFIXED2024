using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Comet.Web.App.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Comet.Web.App.Services;

namespace Comet.Web.App.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> mUserManager;
        private readonly SignInManager<ApplicationUser> mSignInManager;
        private readonly ILogger<LoginModel> mLogger;
        private readonly LanguageService mLanguage;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            LanguageService languageService)
        {
            mUserManager = userManager;
            mSignInManager = signInManager;
            mLogger = logger;
            mLanguage = languageService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<ExternalLoginSchema> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "UserName")]
            [EmailAddress]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "RememberMe")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = "/";
                return Redirect(returnUrl);
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            await SetExternalLoginsAsync();

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            await SetExternalLoginsAsync();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await mSignInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    mLogger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    mLogger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, mLanguage.GetString("InvalidLoginAttempt"));
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SetExternalLoginsAsync()
        {
            ExternalLogins = (await mSignInManager.GetExternalAuthenticationSchemesAsync())
                .Select(x =>
                {
                    switch (x.Name.ToUpper())
                    {
                        case "FACEBOOK":
                            return new ExternalLoginSchema
                            {
                                Icon = "fa fa-facebook",
                                Name = x.Name,
                                DisplayName = x.DisplayName,
                                Class = "btn-primary"
                            };
                        case "GOOGLE":
                            return new ExternalLoginSchema
                            {
                                Icon = "fa fa-google",
                                Name = x.Name,
                                DisplayName = x.DisplayName,
                                Class = "btn-danger"
                            };
                        case "MICROSOFT":
                            return new ExternalLoginSchema
                            {
                                Icon = "fa fa-windows",
                                Name = x.Name,
                                DisplayName = x.DisplayName,
                                Class = "btn-info"
                            };
                        default:
                            return new ExternalLoginSchema
                            {
                                Icon = "fa fa-lock",
                                Name = x.Name,
                                DisplayName = x.DisplayName,
                                Class = "btn-secondary"
                            };
                    }
                })
                .ToList();
        }

        public struct ExternalLoginSchema
        {
            public string Icon;
            public string Name;
            public string DisplayName;
            public string Class;
        }
    }
}
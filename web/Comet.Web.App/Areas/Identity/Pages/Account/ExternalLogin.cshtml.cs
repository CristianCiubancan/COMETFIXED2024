using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Comet.Web.App.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Comet.Web.App.Services;
using Comet.Web.App.Data;
using Microsoft.EntityFrameworkCore;
using Comet.Web.App.Services.Password;

namespace Comet.Web.App.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> mSignInManager;
        private readonly UserManager<ApplicationUser> mUserManager;
        private readonly IEmailSender mEmailSender;
        private readonly ILogger<ExternalLoginModel> mLogger;
        private readonly ApplicationDbContext mDbContext;
        private readonly LanguageService mLanguage;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext dbContext,
            LanguageService language)
        {
            mSignInManager = signInManager;
            mUserManager = userManager;
            mLogger = logger;
            mEmailSender = emailSender;
            mDbContext = dbContext;
            mLanguage = language;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        public Dictionary<string, string> AvailableLanguages = new Dictionary<string, string>();
        public Dictionary<int, string> SecurityQuestions = new Dictionary<int, string>();

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(32, MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(16, MinimumLength = 6)]
            public string SecurityCode { get; set; }

            [Required] public int SecurityQuestion { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(64, MinimumLength = 4)]
            public string SecurityAnswer { get; set; }

            [Required] public string Language { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(64, MinimumLength = 4)]
            public string RealName { get; set; }

            [Required] [DataType(DataType.Date)] public DateTime Birthday { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = mSignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await mSignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await mSignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                mLogger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;

                SecurityQuestions = (await mDbContext.SecurityQuestions.ToListAsync()).ToDictionary(x => x.Id, y => y.Name);
                AvailableLanguages = LanguageService.AvailableLanguages.ToDictionary(x => x.LanguageCultureName, y => y.LanguageFullName);

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    Input = new InputModel
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        RealName = info.Principal.FindFirstValue(ClaimTypes.Name),
                        Phone = info.Principal.FindFirstValue(ClaimTypes.HomePhone)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await mSignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                if ((DateTime.Now - Input.Birthday).TotalDays / 365 < 12)
                {
                    ModelState.AddModelError("Underage", mLanguage.GetString("RegisterUnderage"));
                    return Page();
                }

                if (!ulong.TryParse(Input.SecurityCode, out ulong securityCode))
                {
                    ModelState.AddModelError("SecurityCode", mLanguage.GetString("SecurityCodeInteger"));
                    return Page();
                }

                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                    PasswordHash = string.Empty,
                    Salt = WhirlpoolPasswordService<ApplicationUser>.GenerateSalt(),
                    PhoneNumber = string.Empty,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreationDate = DateTime.Now,
                    SecurityQuestion = Input.SecurityQuestion,
                    SecurityAnswer = Input.SecurityAnswer,
                    SecurityCode = securityCode,
                    Birthdate = Input.Birthday
                };

                var result = await mUserManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    result = await mUserManager.AddLoginAsync(user, info);

                    await mUserManager.AddToRoleAsync(user, Security.Roles.Member);

                    if (result.Succeeded)
                    {
                        mLogger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await mUserManager.GetUserIdAsync(user);
                        var code = await mUserManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId, code },
                            protocol: Request.Scheme);

                        await mEmailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (mUserManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await mSignInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            SecurityQuestions = (await mDbContext.SecurityQuestions.ToListAsync()).ToDictionary(x => x.Id, y => y.Name);
            AvailableLanguages = LanguageService.AvailableLanguages.ToDictionary(x => x.LanguageCultureName, y => y.LanguageFullName);

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
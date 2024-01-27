using Comet.Web.App.Data;
using Comet.Web.App.Data.Entities;
using Comet.Web.App.Middlewares;
using Comet.Web.App.Services;
using Comet.Web.App.Services.Identity;
using Comet.Web.App.Services.Password;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Linq;

namespace Comet.Web.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "PagSeguro",
                    builder =>
                    {
#if DEBUG
                        builder.WithOrigins("https://sandbox.pagseguro.uol.com.br")
#else
                        builder.WithOrigins("https://pagseguro.uol.com.br")
#endif
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                // User settings.
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedAccount = true;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddRoles<ApplicationRole>()
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, int, ApplicationUserRole, ApplicationRoleClaim>>()
                .AddSignInManager<SignInManager>()
                .AddDefaultTokenProviders();

            services.Configure<EmailService.EmailSettings>(opt =>
            {
                opt.PrimaryDomain = Configuration["DontReplyMail:PrimaryDomain"];
                opt.PrimaryPort = int.Parse(Configuration["DontReplyMail:PrimaryPort"]);
                opt.UsernameEmail = Configuration["DontReplyMail:UsernameEmail"];
                opt.UsernamePassword = Configuration["DontReplyMail:UsernamePassword"];
                opt.FromEmail = Configuration["DontReplyMail:FromEmail"];
                opt.EnableSsl = bool.Parse(Configuration["DontReplyMail:EnableSsl"]);
            });
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<LanguageService>();
            services.AddScoped<IPasswordHasher<ApplicationUser>, WhirlpoolPasswordService<ApplicationUser>>();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("Founder", policy =>
                {
                    policy.RequireRole(Security.Roles.SuperAdministrator);
#if !DEBUG
                    policy.RequireClaim("amr", "mfa");
#endif
                });
                opt.AddPolicy("Administrator", policy => policy.RequireRole(Security.Roles.Administrator, Security.Roles.SuperAdministrator));
                opt.AddPolicy("Moderator", policy => policy.RequireRole(Security.Roles.Moderator, Security.Roles.Administrator, Security.Roles.SuperAdministrator));
                opt.AddPolicy("Member", policy => policy.RequireRole(Security.Roles.Member, Security.Roles.Moderator, Security.Roles.Administrator, Security.Roles.SuperAdministrator));
            });

            services.AddAuthentication()
                .AddFacebook(opt =>
                {
                    opt.AppId = Configuration["Authentication:Facebook:AppId"];
                    opt.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(opt =>
                {
                    opt.ClientId = Configuration["Authentication:Google:ClientId"];
                    opt.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                })
                .AddMicrosoftAccount(opt =>
                {
                    opt.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                    opt.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                });

            services.AddControllersWithViews();
#if !DEBUG
            services.AddRazorPages();
            services.AddMvc();
#else
            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("PagSeguro");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCometLocalization();

            app.UseRequestLocalization(opt =>
            {
                opt.DefaultRequestCulture = new RequestCulture(LanguageService.AvailableLanguages[0].LanguageCultureName);
                opt.SupportedCultures = LanguageService.AvailableLanguages.Select(x => new CultureInfo(x.LanguageCultureName)).ToList();
                opt.SupportedUICultures = LanguageService.AvailableLanguages.Select(x => new CultureInfo(x.LanguageCultureName)).ToList();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}

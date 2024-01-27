using Comet.Web.App.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Comet.Web.App.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate mNext;
        private readonly LanguageService mLanguage;
        private IHttpContextAccessor mHttpContextAccessor;

        public LocalizationMiddleware(RequestDelegate next, 
            LanguageService language,
            IHttpContextAccessor httpContextAccessor)
        {
            mNext = next;
            mLanguage = language;
            mHttpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string locale = null;
            if (mHttpContextAccessor.HttpContext?.User != null)
            {
                locale = mHttpContextAccessor.HttpContext.User.FindFirst("Locale")?.Value;
            }

            if (string.IsNullOrEmpty(locale))
            {
                string acceptLanguage = context.Request.Headers["Accept-Language"].ToString().Split(";")[0].Split(",")[0];
                if (mLanguage.IsLanguageAvailable(acceptLanguage))
                    locale = acceptLanguage;
                else
                    locale = mLanguage.CurrentCulture.LanguageCultureName;
            }

            var culture = CultureInfo.CreateSpecificCulture(locale);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            await mNext(context);
        }
    }

    public static class LocalizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCometLocalization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LocalizationMiddleware>();
        }
    }
}

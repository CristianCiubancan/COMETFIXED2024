using System.ComponentModel;
using System.Security.Claims;

namespace Comet.Web.App.Services
{
    public class Security
    {
        public const string SuperAdministrator = "Founder";
        public const string Administrator = "Administrator";
        public const string Moderator = "Moderator";
        public const string Member = "Member";

        public class Roles
        {
            public const string SuperAdministrator = "Super Administrator";
            public const string Administrator = "Administrator";
            public const string Moderator = "Moderator";
            public const string Member = "Member";
        }

        public enum Claims
        {
            Super,
            CreateRoles,
            UpdateRoles,
            DeleteRoles,
            CreateUsers,
            EditUsers,
            DeleteUsers,
            EditRoleClaims,
            EditUserClaims,
            ViewRoles,
            ViewUsers,
            ViewClaims
        }
    }

    public static class SecurityHelper
    {
        public static bool HasPermission(this ClaimsPrincipal principal, Security.Claims claim)
        {
            if (principal.HasClaim(x => x.Type.Equals(Security.Claims.Super.ToPermissionString())))
                return true;
            return principal.HasClaim(x => x.Type.Equals(claim.ToPermissionString()));
        }

        public static string ToPermissionString(this Security.Claims claim)
        {
            return $"Permission.{claim}";
        }
    }
}
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace FNF.ILWeaver.Test.Infrastructure.Extensions
{
    internal static class Identity
    {
        public static void AddOrUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null) return;

            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null) identity.RemoveClaim(existingClaim);

            identity.AddClaim(new Claim(key, value));
        }

        public static string GetClaim(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null) return string.Empty;

            var claim = identity.Claims?.FirstOrDefault(c => c.Type == key);
            return claim?.Value ?? string.Empty;
        }
    }
}
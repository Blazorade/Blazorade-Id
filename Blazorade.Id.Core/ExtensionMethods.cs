using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id
{
    /// <summary>
    /// Defines methods for extending existing classes in Blazorade Id.
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Returns the value of the claim with the specified <paramref name="claimType"/>.
        /// </summary>
        public static string? GetClaimValue(this JwtSecurityToken token, string claimType)
        {
            return token.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value;
        }

        /// <summary>
        /// Returns the expiration time of the token in UTC.
        /// </summary>
        public static DateTime? GetExpirationTimeUtc(this JwtSecurityToken token)
        {
            var exp = token.GetClaimValue("exp");
            if (long.TryParse(exp, out long l))
            {
                var expires = DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime;
                return expires;
            }

            return null;
        }

        /// <summary>
        /// Returns the nonce value from the given <paramref name="token"/>.
        /// </summary>
        public static string? GetNonce(this JwtSecurityToken token)
        {
            return token.GetClaimValue("nonce");
        }

        /// <summary>
        /// Returns the preferred username from the given <paramref name="token"/>.
        /// </summary>
        public static string? GetPreferredUsername(this JwtSecurityToken? token)
        {
            return token?.GetClaimValue("preferred_username");
        }

        /// <summary>
        /// Returns the preferred username from the given <paramref name="principal"/>.
        /// </summary>
        public static string? GetPreferredUsername(this ClaimsPrincipal principal)
        {
            return principal.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }

        /// <summary>
        /// Returns the scopes defined from the given <paramref name="token"/>.
        /// </summary>
        public static IEnumerable<string> GetScopes(this JwtSecurityToken? token)
        {
            if(null != token)
            {
                var scopes = token.GetClaimValue("scp") ?? "";
                return scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }

            return Enumerable.Empty<string>();
        }

    }
}

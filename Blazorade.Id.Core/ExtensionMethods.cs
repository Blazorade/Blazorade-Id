using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks whether the given token contains all of the specified scopes.
        /// </summary>
        /// <param name="token">The token to check for scopes.</param>
        /// <param name="scopes">
        /// <para>
        /// The scopes to require that the token contains.
        /// </para>
        /// <para>
        /// The method returns <see langword="true"/> if the token contains all of the specified scopes,
        /// or if the <paramref name="scopes"/> collection is empty.
        /// </para>
        /// <para>
        /// The method returns <see langword="false"/> if the token does not contain all of the specified scopes.
        /// </para>
        /// </param>
        public static bool ContainsScopes(this JwtSecurityToken token, IEnumerable<string> scopes)
        {
            if(scopes.Count() > 0)
            {
                var scopesClaim = token.Claims.FirstOrDefault(c => c.Type == "scp");
                if (null != scopesClaim && scopesClaim?.Value?.Length > 0)
                {
                    var claimScopes = scopesClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (claimScopes.Intersect(scopes).Count() == scopes.Count())
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

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
        /// Returns the token from the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to get the token from.</param>
        /// <param name="scopes">The scopes that the token must contain.</param>
        /// <returns>The token if it is valid and contains the required scopes; otherwise, <c>null</c>.</returns>
        public static JwtSecurityToken? GetToken(this TokenContainer? container, IEnumerable<string>? scopes)
        {
            if(container?.Expires > DateTime.UtcNow)
            {
                var token = container.ParseToken();
                if(token is not null && token.ContainsScopes(scopes ?? []))
                {
                    return token;
                }
            }
            return null;
        }

        public static bool RequiresInteraction(this Prompt? prompt)
        {
            return prompt.HasValue && (prompt == Prompt.Login || prompt == Prompt.Consent || prompt == Prompt.Select_Account);
        }
    }
}

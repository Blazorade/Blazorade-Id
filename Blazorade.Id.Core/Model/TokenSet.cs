using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    public class TokenSet
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("id_token")]
        public string IdentityToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Returns <c>true</c> if <see cref="AccessToken"/> contains all of the scopes specified in <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope">
        /// A set of scopes to find in the <see cref="AccessToken"/>. Multiple scopes are
        /// separated with a space.
        /// </param>
        /// <returns></returns>
        public bool ContainsScopes(string scope)
        {
            var scopes = scope.Split(" ", options: StringSplitOptions.RemoveEmptyEntries);

            var at = this.GetAccessToken();
            var tokenScopes = at
                ?.Claims
                ?.FirstOrDefault(x => x.Type == "scp")
                ?.Value
                ?.Split(" ", options: StringSplitOptions.RemoveEmptyEntries);
            return tokenScopes?.All(x => scopes.Contains(x)) ?? false;
        }

        public JwtSecurityToken GetAccessToken()
        {
            return new JwtSecurityToken(this.AccessToken);
        }

        public JwtSecurityToken GetIdentityToken()
        {
            return new JwtSecurityToken(this.IdentityToken);
        }
    }
}

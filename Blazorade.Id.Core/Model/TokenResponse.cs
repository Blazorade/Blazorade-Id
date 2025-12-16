using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents a set of tokens received from the token endpoint.
    /// </summary>
    /// <remarks>
    /// A token response is received from the token endpoint when either exchanging an authorization code
    /// for tokens, or when refreshing tokens using a refresh token.
    /// </remarks>
    public class TokenResponse
    {
        /// <summary>
        /// The access token issued by the authorization server.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// The identity token issued by the authorization server.
        /// </summary>
        [JsonPropertyName("id_token")]
        public string? IdentityToken { get; set; }

        /// <summary>
        /// The refresh token issued by the authorization server.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// The type of token issued. Currently, there is only one type: <c>Bearer</c>.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// A space-delimited list of scopes associated with the access token. Only present if the
        /// issued scopes differ from the requested scopes.
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Lifetime of the access token in seconds.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The absolute expiration timestamp of the access token in UTC.
        /// </summary>
        /// <remarks>
        /// This is calculated from the value of <see cref="ExpiresIn"/> when the token response
        /// is received.
        /// </remarks>
        public DateTime ExpiresAtUtc { get; set; }



        /// <summary>
        /// Returns the access token as a <see cref="JwtSecurityToken"/> instance if it exists in the response.
        /// </summary>
        public JwtSecurityToken? GetAccessToken()
        {
            return null != this.AccessToken ? new JwtSecurityToken(this.AccessToken) : null;
        }

        /// <summary>
        /// Returns the identity token as a <see cref="JwtSecurityToken"/> instance if it exists in the response.
        /// </summary>
        public JwtSecurityToken? GetIdentityToken()
        {
            return null != this.IdentityToken ? new JwtSecurityToken(this.IdentityToken) : null;
        }
    }
}

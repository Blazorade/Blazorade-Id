using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    public class TokenRequestSuccess
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("id_token")]
        public string IdToken { get;set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }
    }
}

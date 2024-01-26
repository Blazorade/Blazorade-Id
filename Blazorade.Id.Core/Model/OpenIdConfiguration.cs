using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    public class OpenIdConfiguration
    {
        /// <summary>
        /// The authorization endpoint URI.
        /// </summary>
        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpointUri { get; set; } = string.Empty;

        /// <summary>
        /// The token endpoint URI.
        /// </summary>
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpointUri {  get; set; } = string.Empty;

        /// <summary>
        /// The end session endpoint URI.
        /// </summary>
        [JsonPropertyName("end_session_endpoint")]
        public string EndSessionEndpointUri {  get; set; } = string.Empty;
    }
}

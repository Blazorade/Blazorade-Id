using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Model
{
    public class OpenIdConfiguration
    {
        /// <summary>
        /// The authorization endpoint URI.
        /// </summary>
        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpointUri { get; set; } = string.Empty;

        /// <summary>
        /// The device authorization endpoint URI.
        /// </summary>
        [JsonPropertyName("device_authorization_endpoint")]
        public string DeviceAuthorizationEndpointUri { get; set; } = string.Empty;

        /// <summary>
        /// The Kerberos endpoint URI.
        /// </summary>
        [JsonPropertyName("kerberos_endpoint")]
        public string KerberosEndpointUri { get; set; } = string.Empty;

        /// <summary>
        /// The token endpoint URI.
        /// </summary>
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpointUri {  get; set; } = string.Empty;
        
        /// <summary>
        /// The user info endpoint URI.
        /// </summary>
        [JsonPropertyName("userinfo_endpoint")]
        public string UserInfoEndpointUri {  get; set; } = string.Empty;

        /// <summary>
        /// The end session endpoint URI.
        /// </summary>
        [JsonPropertyName("end_session_endpoint")]
        public string EndSessionEndpointUri {  get; set; } = string.Empty;
    }
}

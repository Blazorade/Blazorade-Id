using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service implementation that is used to resolve various endpoints with.
    /// </summary>
    public class EndpointService : IEndpointService
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        public EndpointService(IHttpClientFactory clientFactory, IOptions<AuthorityOptions> authOptions)
        {
            this.Client = clientFactory.CreateClient();
            this.Options = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly HttpClient Client;
        private readonly AuthorityOptions Options;


        /// <inheritdoc/>
        public async Task<string> GetAuthorizationEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.AuthorizationEndpoint, this.Options.MetadataUri, doc => doc.AuthorizationEndpointUri)
                ?? throw new NullReferenceException("Could not resolve URI for authorization endpoint.");
                ;
        }

        /// <inheritdoc/>
        public async Task<string> GetTokenEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.TokenEndpoint, this.Options.MetadataUri, doc => doc.TokenEndpointUri)
                ?? throw new NullReferenceException("Could not resolve URI for token endpoint.")
                ;
        }

        /// <inheritdoc/>
        public async Task<string?> GetEndSessionEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.EndSessionEndpoint, this.Options.MetadataUri, doc => doc.EndSessionEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetUserInfoEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.MetadataUri, doc => doc.UserInfoEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetDeviceAuthorizationEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.MetadataUri, doc => doc.DeviceAuthorizationEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetKerberosEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.MetadataUri, doc => doc.KerberosEndpointUri);
        }



        private async Task<string?> GetEndpointFromOpenIdConfigurationAsync(string? endpointUri, string? metadataUri, Func<OpenIdConfiguration, string?> uriResolver)
        {
            string? result = null;

            if (endpointUri?.Length > 0)
            {
                result = endpointUri;
            }
            else if(metadataUri?.Length > 0)
            {
                var doc = await this.LoadOpenIdConfigurationAsync(metadataUri);
                if(null != doc)
                {
                    result = uriResolver(doc);
                }
            }

            return result;
        }

        private async Task<OpenIdConfiguration?> LoadOpenIdConfigurationAsync(string metadataUri)
        {
            OpenIdConfiguration? metadata = null!;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(metadataUri)
            };
            var response = await this.Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using (var strm = await response.Content.ReadAsStreamAsync())
                {
                    metadata = await JsonSerializer.DeserializeAsync<OpenIdConfiguration>(await response.Content.ReadAsStreamAsync());
                }
            }

            return metadata;
        }

    }

}

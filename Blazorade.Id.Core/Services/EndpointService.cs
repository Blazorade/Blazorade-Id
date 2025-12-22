using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A service implementation that is used to resolve various endpoints with.
    /// </summary>
    public class EndpointService : IEndpointService
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        public EndpointService(IHttpService httpService, IOptions<AuthorityOptions> authOptions)
        {
            this.HttpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            this.Options = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly IHttpService HttpService;
        private readonly AuthorityOptions Options;


        /// <inheritdoc/>
        public async Task<string> GetAuthorizationEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.AuthorizationEndpoint, this.Options.DiscoveryDocumentUri, doc => doc.AuthorizationEndpointUri)
                ?? throw new NullReferenceException("Could not resolve URI for authorization endpoint.");
                ;
        }

        /// <inheritdoc/>
        public async Task<string> GetTokenEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.TokenEndpoint, this.Options.DiscoveryDocumentUri, doc => doc.TokenEndpointUri)
                ?? throw new NullReferenceException("Could not resolve URI for token endpoint.")
                ;
        }

        /// <inheritdoc/>
        public async Task<string?> GetEndSessionEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.EndSessionEndpoint, this.Options.DiscoveryDocumentUri, doc => doc.EndSessionEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetUserInfoEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.DiscoveryDocumentUri, doc => doc.UserInfoEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetDeviceAuthorizationEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.DiscoveryDocumentUri, doc => doc.DeviceAuthorizationEndpointUri);
        }

        /// <inheritdoc/>
        public async Task<string?> GetKerberosEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(null, this.Options.DiscoveryDocumentUri, doc => doc.KerberosEndpointUri);
        }



        private async Task<string?> GetEndpointFromOpenIdConfigurationAsync(string? endpointUri, string? discoveryDocumentUri, Func<OpenIdDiscoveryDocument, string?> uriResolver)
        {
            string? result = null;

            if (endpointUri?.Length > 0)
            {
                result = endpointUri;
            }
            else if(discoveryDocumentUri?.Length > 0)
            {
                var doc = await this.LoadOpenIdConfigurationAsync(discoveryDocumentUri);
                if(null != doc)
                {
                    result = uriResolver(doc);
                }
            }

            return result;
        }

        private async Task<OpenIdDiscoveryDocument?> LoadOpenIdConfigurationAsync(string discoveryDocumentUri)
        {
            OpenIdDiscoveryDocument? metadata = null!;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(discoveryDocumentUri)
            };
            var response = await this.HttpService.SendRequestAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using (var strm = await response.Content.ReadAsStreamAsync())
                {
                    metadata = await JsonSerializer.DeserializeAsync<OpenIdDiscoveryDocument>(await response.Content.ReadAsStreamAsync());
                }
            }

            return metadata;
        }

    }

}

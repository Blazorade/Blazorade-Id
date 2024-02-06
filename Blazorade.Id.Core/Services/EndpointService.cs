using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
    public class EndpointService
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="clientFactory">The HTTP client factory that is a dependency to this service instance.</param>
        public EndpointService(IHttpClientFactory? clientFactory, CodeChallengeService codeChallengeService)
        {
            this.Client = clientFactory?.CreateClient() ?? new HttpClient();
            this.CodeChallenge = codeChallengeService;
        }

        private readonly HttpClient Client;
        private readonly CodeChallengeService CodeChallenge;

        /// <summary>
        /// Creates an endpoint builder that is used to build the authorization endpoint URI with.
        /// </summary>
        /// <param name="options">The options to use when resolving the authorization endpoint URI for the configured authority.</param>
        /// <returns>
        /// The builder returned is configured with the <see cref="AuthorityOptions.ClientId"/> from <paramref name="options"/>.
        /// </returns>
        /// <exception cref="Exception">The exception that is thrown when the authorization endpoint URI could not be resolved.</exception>
        public async ValueTask<EndpointUriBuilder> CreateAuthorizationUriBuilderAsync(AuthorityOptions options)
        {
            var uri = await this.GetAuthorizationEndpointAsync(options) ?? throw new Exception("Could not resolve URI for authorization endpoint.");
            return new EndpointUriBuilder(uri, this.CodeChallenge).WithClientId(options.ClientId);
        }

        public async ValueTask<EndpointUriBuilder> CreateEndSessionUriBuilderAsync(AuthorityOptions options)
        {
            var uri = await this.GetEndSessionEndpointAsync(options) ?? throw new Exception("Could not resolve URI for end session endpoint.");
            return new EndpointUriBuilder(uri, this.CodeChallenge);
        }

        public async ValueTask<TokenRequestBuilder> CreateTokenRequestBuilderAsync(AuthorityOptions options)
        {
            var uri = await this.GetTokenEndpointAsync(options) ?? throw new Exception("Could not resolve URI for token endpoint");
            return new TokenRequestBuilder(uri).WithClientId(options.ClientId);
        }

        public async ValueTask<string?> GetAuthorizationEndpointAsync(AuthorityOptions options)
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(options.AuthorizationEndpoint, options.MetadataUri, doc => doc.AuthorizationEndpointUri);
        }

        public async ValueTask<string?> GetTokenEndpointAsync(AuthorityOptions options)
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(options.TokenEndpoint, options.MetadataUri, doc => doc.TokenEndpointUri);
        }

        public async ValueTask<string?> GetEndSessionEndpointAsync(AuthorityOptions options)
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(options.EndSessionEndpoint, options.MetadataUri, doc => doc.EndSessionEndpointUri);
        }



        private async ValueTask<string?> GetEndpointFromOpenIdConfigurationAsync(string? endpointUri, string? metadataUri, Func<OpenIdConfiguration, string?> uriResolver)
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

        private async ValueTask<OpenIdConfiguration?> LoadOpenIdConfigurationAsync(string metadataUri)
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

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
        public EndpointService(IHttpClientFactory? clientFactory, CodeChallengeService codeChallengeService, IOptions<AuthorityOptions> authOptions)
        {
            this.Client = clientFactory?.CreateClient() ?? new HttpClient();
            this.CodeChallenge = codeChallengeService;
            this.Options = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly HttpClient Client;
        private readonly CodeChallengeService CodeChallenge;
        private readonly AuthorityOptions Options;

        /// <summary>
        /// Creates an endpoint builder that is used to build the authorization endpoint URI with.
        /// </summary>
        /// <param name="options">The options to use when resolving the authorization endpoint URI for the configured authority.</param>
        /// <returns>
        /// The builder returned is configured with the <see cref="AuthorityOptions.ClientId"/> from <paramref name="options"/>.
        /// </returns>
        /// <exception cref="Exception">The exception that is thrown when the authorization endpoint URI could not be resolved.</exception>
        public async ValueTask<EndpointUriBuilder> CreateAuthorizationUriBuilderAsync()
        {
            var uri = await this.GetAuthorizationEndpointAsync() ?? throw new Exception("Could not resolve URI for authorization endpoint.");
            return new EndpointUriBuilder(uri, this.CodeChallenge).WithClientId(this.Options.ClientId);
        }

        /// <summary>
        /// Returns an URI Builder that can be used to build URIs to the the end session endpoint for the configured authority.
        /// </summary>
        /// <exception cref="Exception">
        /// The exception that is thrown if the end session endpoint could not be resolved.
        /// </exception>
        public async ValueTask<EndpointUriBuilder> CreateEndSessionUriBuilderAsync()
        {
            var uri = await this.GetEndSessionEndpointAsync() ?? throw new Exception("Could not resolve URI for end session endpoint.");
            return new EndpointUriBuilder(uri, this.CodeChallenge);
        }

        /// <summary>
        /// Creates an URI Builder that is used to build URIs to the token endpoint for the configured authority.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// The exception that is thrown if the token endpoint could not be resolved.
        /// </exception>
        public async ValueTask<TokenRequestBuilder> CreateTokenRequestBuilderAsync()
        {
            var uri = await this.GetTokenEndpointAsync() ?? throw new Exception("Could not resolve URI for token endpoint");
            return new TokenRequestBuilder(uri).WithClientId(this.Options.ClientId);
        }

        /// <summary>
        /// Returns the authorization endpoint for the configured authority.
        /// </summary>
        public async ValueTask<string?> GetAuthorizationEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.AuthorizationEndpoint, this.Options.MetadataUri, doc => doc.AuthorizationEndpointUri);
        }

        /// <summary>
        /// Returns the token endpoint for the configured authority.
        /// </summary>
        public async ValueTask<string?> GetTokenEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.TokenEndpoint, this.Options.MetadataUri, doc => doc.TokenEndpointUri);
        }

        /// <summary>
        /// Returns the end session endpoint for the configured authority.
        /// </summary>
        public async ValueTask<string?> GetEndSessionEndpointAsync()
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(this.Options.EndSessionEndpoint, this.Options.MetadataUri, doc => doc.EndSessionEndpointUri);
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

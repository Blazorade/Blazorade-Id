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
    public class EndpointService
    {
        public EndpointService(IHttpClientFactory? clientFactory)
        {
            this.Client = clientFactory?.CreateClient() ?? new HttpClient();
        }

        private readonly HttpClient Client;

        public async ValueTask<EndpointUriBuilder> CreateAuthorizationUriBuilderAsync(AuthenticationOptions options)
        {
            var uri = await this.GetAuthorizationEndpointAsync(options) ?? throw new Exception("Could not resolve URI for authorization endpoint.");
            return new EndpointUriBuilder(uri);
        }

        public async ValueTask<TokenRequestBuilder> CreateTokenRequestBuilderAsync(AuthenticationOptions options)
        {
            var uri = await this.GetTokenEndpointAsync(options) ?? throw new Exception("Could not resolve URI for token endpoint");
            return new TokenRequestBuilder(uri);
        }
        public async ValueTask<string?> GetAuthorizationEndpointAsync(AuthenticationOptions options)
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(options.AuthorizationEndpoint, options.MetadataUri, doc => doc.AuthorizationEndpointUri);
        }

        public async ValueTask<string?> GetTokenEndpointAsync(AuthenticationOptions options)
        {
            return await this.GetEndpointFromOpenIdConfigurationAsync(options.TokenEndpoint, options.MetadataUri, doc => doc.TokenEndpointUri);
        }

        public async ValueTask<string?> GetEndSessionEndpointAsync(AuthenticationOptions options)
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

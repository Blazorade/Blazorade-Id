using Blazorade.Id.Core.Configuration;
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

        public async Task<string?> GetAuthorizationEndpointAsync(BlazoradeAuthenticationOptions options)
        {
            return await this.GetEndpointFromMetadataDocumentAsync(options.AuthorizationEndpoint, options.MetadataUri, doc => doc.authorization_endpoint);
        }

        public async Task<string?> GetTokenEndpointAsync(BlazoradeAuthenticationOptions options)
        {
            return await this.GetEndpointFromMetadataDocumentAsync(options.TokenEndpoint, options.MetadataUri, doc => doc.token_endpoint);
        }

        public async Task<string?> GetEndSessionEndpointAsync(BlazoradeAuthenticationOptions options)
        {
            return await this.GetEndpointFromMetadataDocumentAsync(options.EndSessionEndpoint, options.MetadataUri, doc => doc.end_session_endpoint);
        }



        private async Task<string?> GetEndpointFromMetadataDocumentAsync(string? endpointUri, string? metadataUri, Func<MetadataDocument, string?> uriResolver)
        {
            string? result = null;

            if (endpointUri?.Length > 0)
            {
                result = endpointUri;
            }
            else if(metadataUri?.Length > 0)
            {
                var doc = await this.LoadMetadataDocumentAsync(metadataUri);
                if(null != doc)
                {
                    result = uriResolver(doc);
                }
            }

            return result;
        }

        private Dictionary<string, MetadataDocument> MetaDocs = new Dictionary<string, MetadataDocument>();
        private async Task<MetadataDocument?> LoadMetadataDocumentAsync(string metadataUri)
        {
            MetadataDocument? metadata = null!;
            if(this.MetaDocs.ContainsKey(metadataUri))
            {
                metadata = this.MetaDocs[metadataUri];
            }
            else
            {
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
                        metadata = await JsonSerializer.DeserializeAsync<MetadataDocument>(await response.Content.ReadAsStreamAsync());
                    }
                }

                if(null != metadata)
                {
                    this.MetaDocs[metadataUri] = metadata;
                }
            }

            return metadata;
        }



        private class MetadataDocument
        {

            public string authorization_endpoint { get; set; } = null!;

            public string token_endpoint { get; set; } = null!;

            public string end_session_endpoint { get; set; } = null!;
        }
    }

}

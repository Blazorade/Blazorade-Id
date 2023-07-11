using Blazorade.Authentication.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Authentication.Services
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
            if(options?.AuthorizationEndpoint?.Length > 0)
            {
                return options.AuthorizationEndpoint;
            }
            else if(options?.MetadataUri?.Length > 0)
            {
                var doc = await this.LoadMetadataDocumentAsync(options.MetadataUri);
                return doc.authorization_endpoint;
            }
            return null;
        }

        public async Task<string?> GetTokenEndpointAsync(BlazoradeAuthenticationOptions options)
        {
            if(options?.TokenEndpoint?.Length > 0)
            {
                return options.TokenEndpoint;
            }
            else if(options?.MetadataUri?.Length > 0)
            {
                var doc = await this.LoadMetadataDocumentAsync(options.MetadataUri);
                return doc.token_endpoint;
            }
            return null;
        }


        private async Task<MetadataDocument?> LoadMetadataDocumentAsync(string metadataUri)
        {
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get, 
                RequestUri = new Uri(metadataUri)
            };
            var response = await this.Client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                var doc = await JsonSerializer.DeserializeAsync<MetadataDocument>(response.Content.ReadAsStream());
                return doc;
            }

            return null;
        }



        private class MetadataDocument
        {

            public string authorization_endpoint { get; set; } = null!;

            public string token_endpoint { get; set; } = null!;

        }
    }

}

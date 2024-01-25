using Blazorade.Id.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class TokenRequestBuilder : BuilderBase<HttpRequestMessage>
    {
        public TokenRequestBuilder(string tokenEndpointUri, IHttpClientFactory? clientFactory = null)
        {
            this.TokenEndpointUri = tokenEndpointUri;
            this.Client = clientFactory?.CreateClient() ?? new HttpClient();
        }

        private readonly string TokenEndpointUri;
        private readonly HttpClient Client;

        public static async Task<TokenRequestBuilder> CreateAsync(BlazoradeAuthenticationOptions options, IHttpClientFactory? clientFactory = null)
        {
            var epService = new EndpointService(clientFactory);
            var uri = await epService.GetTokenEndpointAsync(options);
            if(uri?.Length > 0)
            {
                return new TokenRequestBuilder(uri, clientFactory);
            }

            throw new Exception("Unable to resolve token endpoint URI.");
        }


        public override HttpRequestMessage Build()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.TokenEndpointUri)
            {
                
            };

            return request;
        }
    }
}

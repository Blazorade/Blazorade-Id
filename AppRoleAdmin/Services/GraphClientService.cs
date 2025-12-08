using AppRoleAdmin.Security;
using Azure.Core;
using Blazorade.Id.Core.Services;
using Microsoft.Graph;
using Microsoft.Graph.Applications;
using Microsoft.Graph.Models;

namespace AppRoleAdmin.Services
{
    public class GraphClientService
    {
        public GraphClientService(ITokenService tokenService, IHttpClientFactory httpFactory)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.HttpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
        }

        private readonly ITokenService TokenService;
        private readonly IHttpClientFactory HttpFactory;

        public async IAsyncEnumerable<Application> GetApplicationsAsync(string? query)
        {
            var graph = await this.GetGraphClientAsync(scope: Scopes.ApplicationReadAll);
            if(null != graph)
            {
                var appsResponse = await graph.Applications.GetAsync(requestConfiguration: config =>
                {
                    config.Headers.Add("ConsistencyLevel", "eventual");
                    config.QueryParameters.Count = true;

                    if(query?.Length > 0)
                    {
                        config.QueryParameters.Search = $"\"displayName:{query}\"";
                    }
                });

                foreach(var app in appsResponse?.Value ?? [])
                {
                    yield return app;
                }
            }

            yield break;
        }


        private async Task<GraphServiceClient?> GetGraphClientAsync(string? scope)
        {
            var token = await this.TokenService.GetAccessTokenAsync(options: new GetTokenOptions { Scopes = scope != null ? new[] { scope } : null });

            if(null != token)
            {
                var httpClient = this.HttpFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.RawData);

                var graphClient = new GraphServiceClient(httpClient);
                return graphClient;
            }

            return null;
        }
    }
}

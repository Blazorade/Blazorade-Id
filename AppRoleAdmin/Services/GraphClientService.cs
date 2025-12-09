using Azure.Core;
using Blazorade.Id.Core.Configuration;
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


        public async Task<Application?> GetApplicationAsync(string? id)
        {
            if(id?.Length > 0)
            {
                var graph = await this.GetGraphClientAsync(scope: Scopes.ApplicationReadAll);
                if (null != graph)
                {
                    var app = await graph.Applications[id].GetAsync(config =>
                    {
                        config.QueryParameters.Select = ["id", "appId", "displayName", "appRoles"];
                    });
                    return app;
                }
            }

            return null;
        }
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
                    if(null != app)
                    {
                        yield return app;
                    }
                }
            }

            yield break;
        }

        public async Task<ServicePrincipal?> GetServicePrincipalAsync(Application app)
        {
            var graph = await this.GetGraphClientAsync(scope: Scopes.ApplicationReadAll);
            if(null != graph)
            {
                var response = await graph.ServicePrincipals.GetAsync(config =>
                {
                    config.QueryParameters.Filter = $"appId eq '{app.AppId}'";
                });

                return response?.Value?.FirstOrDefault();
            }

            return null;
        }

        public async IAsyncEnumerable<AppRoleAssignment> GetAppRoleAssignmentsAsync(ServicePrincipal principal, AppRole role)
        {
            var graph = await this.GetGraphClientAsync(scope: $"{Scopes.ApplicationReadAll} {Scopes.DirectoryReadAll}");
            if(null != graph)
            {
                var response = await graph.ServicePrincipals[principal.Id].AppRoleAssignedTo.GetAsync(config =>
                {
                    
                });
                while(null != response)
                {
                    foreach (var assignment in response.Value ?? [])
                    {
                        if (null != assignment && assignment.AppRoleId == role.Id)
                        {
                            yield return assignment;
                        }
                    }

                    if(response.OdataNextLink?.Length > 0)
                    {
                        response = await graph.ServicePrincipals[principal.Id]
                            .AppRoleAssignments
                            .WithUrl(response.OdataNextLink)
                            .GetAsync();
                    }
                    else
                    {
                        response = null;
                    }
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


        public static class Scopes
        {
            public const string ApplicationReadAll = "Application.Read.All";
            public const string ApplicationReadWriteAll = "Application.ReadWrite.All";
            public const string DirectoryReadAll = "Directory.Read.All";
        }
    }
}

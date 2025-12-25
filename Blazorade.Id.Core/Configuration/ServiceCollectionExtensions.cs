using Blazorade.Id.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Configuration
{
    /// <summary>
    /// Extension methods for registering Blazorade Id services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the default Blazorade Id services that are shared across all Blazorade Id application types.
        /// </summary>
        public static IServiceCollection AddBlazoradeIdSharedServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IEndpointService, EndpointService>()
                .AddScoped<ICodeChallengeService, CodeChallengeService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IRefreshTokenStore, NullRefreshTokenStore>()
                .AddScoped<IAuthorizationCodeProcessor, AuthorizationCodeProcessor>()
                .AddScoped<IScopeSorter, ScopeSorter>()
                .AddScoped<ITokenStore, InMemoryTokenStore>()
                .AddScoped<IPropertyStore, InMemoryPropertyStore>()
                .AddScoped<ITokenRefresher, TokenRefresher>()
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<IAuthorizationCodeFailureNotifier, AuthorizationCodeFailureNotifier>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IHttpRequestFactory, HttpRequestFactory>()
                .AddHttpClient()

                .AddOptions<JsonSerializerOptions>()
                .Configure(opt =>
                {
                    opt.PropertyNameCaseInsensitive = true;
                    opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }).Services
                ;
        }

    }
}

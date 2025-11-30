using Blazorade.Id.Components;
using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Blazorade.Id.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static BlazoradeIdBuilder AddBlazoradeIdServerApplication(this IServiceCollection services)
        {
            return new BlazoradeIdBuilder(
                services
                    .AddSharedBlazoradeWebServices()
            );
        }

        public static BlazoradeIdBuilder AddBlazoradeIdWasmApplication(this IServiceCollection services)
        {
            return new BlazoradeIdBuilder(
                services
                    .AddSharedBlazoradeWebServices()
            );
        }



        private static IServiceCollection AddSharedBlazoradeWebServices(this IServiceCollection services)
        {
            return services
                .AddAuthorizationCore()
                .AddCascadingAuthenticationState()
                .AddScoped<AuthenticationStateProvider, BlazoradeIdAuthenticationStateProvider>()
                .AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp =>
                {
                    var stateProvider = sp.GetRequiredService<AuthenticationStateProvider>();
                    return (IHostEnvironmentAuthenticationStateProvider)stateProvider;
                })
                .AddBlazoredSessionStorage()
                .AddBlazoredLocalStorage()
                .AddSharedBlazoradeIdServices()
                .AddScoped<IAuthCodeProvider, BlazorAuthCodeProvider>()
                .AddScoped<IPropertyStore, BlazorSessionPropertyStore>()
                .AddScoped<ITokenStore, InMemoryTokenStore>()
                .AddScoped<IAuthenticationStateNotifier, BlazorAuthenticationStateNotifier>()
                ;
        }

        private static IServiceCollection AddSharedBlazoradeIdServices(this IServiceCollection services)
        {
            return services
                .AddScoped<EndpointService>()
                .AddScoped<ICodeChallengeService, CodeChallengeService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IAuthCodeProcessor, AuthCodeProcessor>()
                .AddHttpClient()

                .AddOptions<JsonSerializerOptions>()
                .Configure(opt =>
                {
                    opt.PropertyNameCaseInsensitive = true;
                    opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }).Services
                ;
            ;
        }
    }
}

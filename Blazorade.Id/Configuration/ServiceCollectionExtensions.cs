using Blazorade.Id.Components;
using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Blazorade.Id.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
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
    /// <summary>
    /// Provides extension methods for registering Blazorade.Id services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds all necessary services that are needed to use Blazorade Id in a Blazor Server application.
        /// </summary>
        /// <remarks>
        /// Use the returned <see cref="BlazoradeIdBuilder"/> to further configure the Blazorade Id services.
        /// </remarks>
        public static BlazoradeIdBuilder AddBlazoradeIdServerApplication(this IServiceCollection services)
        {
            return new BlazoradeIdBuilder(
                services
                    .AddSharedBlazoradeWebServices()
            );
        }

        /// <summary>
        /// Adds all necessary services that are needed to use Blazorade Id in a Blazor WebAssembly application.
        /// </summary>
        /// <remarks>
        /// Use the returned <see cref="BlazoradeIdBuilder"/> to further configure the Blazorade Id services.
        /// </remarks>
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
                .AddScoped<BlazoradeIdScriptService>()

                // The following service is used when configuring options, so it has to be registered as singleton,
                // since options are always singleton.
                .AddSingleton<IRedirectUriProvider, BlazorRedirectUriProvider>()
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

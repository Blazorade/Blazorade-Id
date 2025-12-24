using Blazorade.Id.Components;
using Blazorade.Id.Configuration;
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



        /// <summary>
        /// Adds default Blazorade Id services that are shared across all Blazor web application types.
        /// </summary>
        private static IServiceCollection AddSharedBlazoradeWebServices(this IServiceCollection services)
        {
            return services
                .AddAuthorizationCore()
                .AddCascadingAuthenticationState()
                .AddBlazoradeIdSharedServices()
                .AddScoped<AuthenticationStateProvider, BlazoradeIdAuthenticationStateProvider>()
                .AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp =>
                {
                    var stateProvider = sp.GetRequiredService<AuthenticationStateProvider>();
                    return (IHostEnvironmentAuthenticationStateProvider)stateProvider;
                })
                .AddBlazoredSessionStorage()
                .AddBlazoredLocalStorage()
                .AddScoped<IRedirectUriProvider, BlazorRedirectUriProvider>()
                .AddScoped<IAuthorizationCodeProvider, BlazorAuthCodeProvider>()
                .AddScoped<IPropertyStore, BrowserSessionStoragePropertyStore>()
                .AddScoped<IAuthenticationStateNotifier, BlazorAuthenticationStateNotifier>()
                .AddScoped<BlazoradeIdScriptService>()
                .AddScoped<IAuthenticationService, BlazorAuthenticationService>()
                ;
        }

    }
}

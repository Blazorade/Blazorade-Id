using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Components;
using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static BlazoradeIdBuilder AddBlazoradeIdServerApplication(this IServiceCollection services)
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
                .AddBlazoradeId()
                .AddStorage<BlazorSessionStorage, BlazorPersistentStorage>()
                .AddNavigator<BlazorNavigator>()
                ;
        }

        public static BlazoradeIdBuilder AddBlazoradeIdWasmApplication(this IServiceCollection services)
        {
            return services
                .AddBlazoradeIdServerApplication()
                ;
        }
    }
}

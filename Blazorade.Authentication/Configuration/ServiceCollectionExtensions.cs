using Blazorade.Authentication.Configuration;
using Blazorade.Authentication.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddBlazoradeAuthentication(this IServiceCollection services)
        {
            return services
                .AddScoped<BlazoradeAuthenticationService>()
                .AddScoped<StorageProxy>()
                .AddBlazoredLocalStorage()
                .AddBlazoredSessionStorage()
                ;
        }

        public static IServiceCollection AddBlazoradeAuthentication(this IServiceCollection services, Action<BlazoradeAuthenticationOptions> config)
        {
            return services
                .AddBlazoradeAuthentication()
                .AddBlazoradeAuthentication((sp, options) =>
                {
                    config?.Invoke(options);
                });
        }

        public static IServiceCollection AddBlazoradeAuthentication(this IServiceCollection services, Action<IServiceProvider, BlazoradeAuthenticationOptions> config)
        {
            return services
                .AddBlazoradeAuthentication()
                .AddOptions<BlazoradeAuthenticationOptions>()
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                }).Services;
        }
    }
}

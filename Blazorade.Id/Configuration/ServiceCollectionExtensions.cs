using Blazorade.Id.Configuration;
using Blazorade.Id.Services;
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

        public static IServiceCollection AddBlazoradeId(this IServiceCollection services)
        {
            return services
                .AddScoped<BlazoradeAuthenticationService>()
                .AddScoped<StorageService>()
                .AddSingleton<EndpointService>()
                .AddBlazoradeCore()
                .AddBlazoredLocalStorage()
                .AddBlazoredSessionStorage()
                ;
        }

        public static IServiceCollection AddBlazoradeId(this IServiceCollection services, Action<BlazoradeAuthenticationOptions> config)
        {
            return services
                .AddBlazoradeId()
                .AddBlazoradeId((sp, options) =>
                {
                    config?.Invoke(options);
                });
        }

        public static IServiceCollection AddBlazoradeId(this IServiceCollection services, Action<IServiceProvider, BlazoradeAuthenticationOptions> config)
        {
            return services
                .AddBlazoradeId()
                .AddOptions<BlazoradeAuthenticationOptions>()
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                }).Services;
        }

        public static IServiceCollection AddBlazoradeId(this IServiceCollection services, string optionsName, Action<IServiceProvider, BlazoradeAuthenticationOptions> config)
        {
            return services
                .AddBlazoradeId()
                .AddOptions<BlazoradeAuthenticationOptions>(optionsName)
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                }).Services;
        }
    }
}

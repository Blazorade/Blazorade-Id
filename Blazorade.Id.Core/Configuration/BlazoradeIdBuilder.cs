using Blazorade.Id.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Blazorade.Id.Core.Configuration
{
    /// <summary>
    /// A builder class that you use to further configure your application.
    /// </summary>
    public class BlazoradeIdBuilder
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazoradeIdBuilder"/> class.
        /// </summary>
        /// <param name="services"></param>
        public BlazoradeIdBuilder(IServiceCollection services)
        {
            this.Services = this.AddBlazoradeIdSharedServices(services);
        }

        /// <summary>
        /// The service collection that this builder adds services to.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Adds authority configuration to the application.
        /// </summary>
        public BlazoradeIdBuilder AddAuthority(Action<IServiceProvider, AuthorityOptions> config)
        {
            this.Services.AddOptions<AuthorityOptions>()
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                });
            return this;
        }

        /// <summary>
        /// Adds the authentication state notifier used in the application.
        /// </summary>
        /// <typeparam name="TAuthenticationStateNotifier">The type of authentication state notifier to use.</typeparam>
        /// <remarks>
        /// An authentication state notifier is responsible for notifying the application when the authentication
        /// state of the user has changed. This typically occurs when the user logs in, or changes the user account
        /// that they are logged in to the application with.
        /// </remarks>
        public BlazoradeIdBuilder AddAuthenticationStateNotifier<TAuthenticationStateNotifier>() where TAuthenticationStateNotifier : class, IAuthenticationStateNotifier
        {
            this.Services.AddScoped<IAuthenticationStateNotifier, TAuthenticationStateNotifier>();
            return this;
        }

        /// <summary>
        /// Adds the authentication state notifier used in the application.
        /// </summary>
        /// <param name="config">The delegate that creates the authentication state notifier to use in the application.</param>
        /// <remarks>
        /// An authentication state notifier is responsible for notifying the application when the authentication
        /// state of the user has changed. This typically occurs when the user logs in, or changes the user account
        /// that they are logged in to the application with.
        /// </remarks>
        public BlazoradeIdBuilder AddAuthenticationStateNotifier(Func<IServiceProvider, IAuthenticationStateNotifier> config)
        {
            this.Services.AddScoped<IAuthenticationStateNotifier>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the property storage used in the application.
        /// </summary>
        /// <typeparam name="TPropertyStore">The type of property storage to add.</typeparam>
        public BlazoradeIdBuilder AddPropertyStore<TPropertyStore>() where TPropertyStore : class, IPropertyStore
        {
            this.Services.AddScoped<IPropertyStore, TPropertyStore>();
            return this;
        }

        /// <summary>
        /// Adds the property storage to use in the application.
        /// </summary>
        public BlazoradeIdBuilder AddPropertyStore(Func<IServiceProvider, IPropertyStore> config)
        {
            this.Services.AddScoped<IPropertyStore>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the scope sorter used in the application.
        /// </summary>
        /// <typeparam name="TScopeSorter">The type of scope sorter to use.</typeparam>
        /// <remarks>
        /// A default scope sorter is provided and registered by Blazorade Id, but you can implement
        /// your own scope sorter by implementing the <see cref="IScopeSorter"/> interface.
        /// </remarks>
        public BlazoradeIdBuilder AddScopeSorter<TScopeSorter>() where TScopeSorter : class, IScopeSorter
        {
            this.Services.AddScoped<IScopeSorter, TScopeSorter>();
            return this;
        }

        /// <summary>
        /// Adds the scope sorter used in the application.
        /// </summary>
        /// <remarks>
        /// A default scope sorter is provided and registered by Blazorade Id, but you can implement
        /// your own scope sorter by implementing the <see cref="IScopeSorter"/> interface.
        /// </remarks>
        public BlazoradeIdBuilder AddScopeSorter(Func<IServiceProvider, IScopeSorter> config)
        {
            this.Services.AddScoped<IScopeSorter>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the token store to use in the application.
        /// </summary>
        /// <typeparam name="TTokenStore">The token store implementation to add.</typeparam>
        public BlazoradeIdBuilder AddTokenStore<TTokenStore>() where TTokenStore : class, ITokenStore
        {
            this.Services.AddScoped<ITokenStore, TTokenStore>();
            return this;
        }

        /// <summary>
        /// Adds the token store to use in the application.
        /// </summary>
        /// <param name="config">The delegate that configures the token store.</param>
        /// <returns></returns>
        public BlazoradeIdBuilder AddTokenStore(Func<IServiceProvider, ITokenStore> config)
        {
            this.Services.AddScoped<ITokenStore>(sp => config.Invoke(sp));
            return this;
        }



        /// <summary>
        /// Adds the default Blazorade Id services that are shared across all Blazorade Id application types.
        /// </summary>
        private IServiceCollection AddBlazoradeIdSharedServices(IServiceCollection services)
        {
            return services
                .AddScoped<EndpointService>()
                .AddScoped<ICodeChallengeService, CodeChallengeService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IAuthCodeProcessor, AuthCodeProcessor>()
                .AddScoped<IScopeSorter, DefaultScopeSorter>()
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

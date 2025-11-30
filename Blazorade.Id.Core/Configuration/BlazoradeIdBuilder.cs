using Blazorade.Id.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Configuration
{
    /// <summary>
    /// A builder class that you use to further configure your application.
    /// </summary>
    public class BlazoradeIdBuilder
    {
        public BlazoradeIdBuilder(IServiceCollection services)
        {
            this.Services = services;
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
        /// <typeparam name="TPropertyStorage">The type of property storage to add.</typeparam>
        public BlazoradeIdBuilder AddPropertyStorage<TPropertyStorage>() where TPropertyStorage : class, IPropertyStore
        {
            this.Services.AddScoped<IPropertyStore, TPropertyStorage>();
            return this;
        }

        /// <summary>
        /// Adds the property storage to use in the application.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public BlazoradeIdBuilder AddPropertyStorage(Func<IServiceProvider, IPropertyStore> config)
        {
            this.Services.AddScoped<IPropertyStore>(sp => config.Invoke(sp));
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

    }
}

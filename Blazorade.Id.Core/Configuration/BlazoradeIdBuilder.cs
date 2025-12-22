using Blazorade.Id.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Blazorade.Id.Configuration
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
        /// Adds the authorization code failure notifier used in the application.
        /// </summary>
        /// <typeparam name="TAuthorizationCodeFailureNotifier">The type of authorization code failure notifier to use.</typeparam>
        public BlazoradeIdBuilder AddAuthorizationCodeFailureNotifier<TAuthorizationCodeFailureNotifier>() where TAuthorizationCodeFailureNotifier : class, IAuthorizationCodeFailureNotifier
        {
            this.Services.AddScoped<IAuthorizationCodeFailureNotifier, TAuthorizationCodeFailureNotifier>();
            return this;
        }

        /// <summary>
        /// Adds the authorization code failure notifier used in the application.
        /// </summary>
        public BlazoradeIdBuilder AddAuthorizationCodeFailureNotifier(Func<IServiceProvider, IAuthorizationCodeFailureNotifier> config)
        {
            this.Services.AddScoped<IAuthorizationCodeFailureNotifier>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the authorization code processor used in the application.
        /// </summary>
        /// <typeparam name="TAuthorizationCodeProcessor">The type of authorization code processor to use.</typeparam>
        public BlazoradeIdBuilder AddAuthorizationCodeProcessor<TAuthorizationCodeProcessor>() where TAuthorizationCodeProcessor : class, IAuthorizationCodeProcessor
        {
            this.Services.AddScoped<IAuthorizationCodeProcessor, TAuthorizationCodeProcessor>();
            return this;
        }

        /// <summary>
        /// Adds the authorization code processor used in the application.
        /// </summary>
        public BlazoradeIdBuilder AddAuthorizationCodeProcessor(Func<IServiceProvider, IAuthorizationCodeProcessor> config)
        {
            this.Services.AddScoped<IAuthorizationCodeProcessor>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the authorization code provider used in the application.
        /// </summary>
        /// <typeparam name="TAuthorizationCodeProvider">The type of authorization code provider to use.</typeparam>
        public BlazoradeIdBuilder AddAuthorizationCodeProvider<TAuthorizationCodeProvider>() where TAuthorizationCodeProvider : class, IAuthorizationCodeProvider
        {
            this.Services.AddScoped<IAuthorizationCodeProvider, TAuthorizationCodeProvider>();
            return this;
        }
        
        /// <summary>
        /// Adds the authorization code provider used in the application.
        /// </summary>
        public BlazoradeIdBuilder AddAuthorizationCodeProvider(Func<IServiceProvider, IAuthorizationCodeProvider> config)
        {
            this.Services.AddScoped<IAuthorizationCodeProvider>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the endpoint service used in the application.
        /// </summary>
        /// <typeparam name="TEndpointService">The type of endpoint service.</typeparam>
        public BlazoradeIdBuilder AddEndpointService<TEndpointService>() where TEndpointService : class, IEndpointService
        {
            this.Services.AddScoped<IEndpointService, TEndpointService>();
            return this;
        }

        /// <summary>
        /// Adds the endpoint service used in the application.
        /// </summary>
        public BlazoradeIdBuilder AddEndpointService(Func<IServiceProvider, IEndpointService> config)
        {
            this.Services.AddScoped<IEndpointService>(sp => config.Invoke(sp));
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
        /// Adds the redirect URI provider used in the application.
        /// </summary>
        /// <typeparam name="TRedirectUriProvider">The type of redirect URI provider to use.</typeparam>
        public BlazoradeIdBuilder AddRedirectUriProvider<TRedirectUriProvider>() where TRedirectUriProvider : class, IRedirectUriProvider
        {
            this.Services.AddScoped<IRedirectUriProvider, TRedirectUriProvider>();
            return this;
        }

        /// <summary>
        /// Adds the redirect URI provider used in the application.
        /// </summary>
        public BlazoradeIdBuilder AddRedirectUriProvider(Func<IServiceProvider, IRedirectUriProvider> config)
        {
            this.Services.AddScoped<IRedirectUriProvider>(sp => config.Invoke(sp));
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
        /// Adds the sign-out service to use in the application.
        /// </summary>
        /// <typeparam name="TSignOutService">The type of sign-out service.</typeparam>
        public BlazoradeIdBuilder AddSignOutService<TSignOutService>() where TSignOutService : class, ISignOutService
        {
            this.Services.AddScoped<ISignOutService, TSignOutService>();
            return this;
        }

        /// <summary>
        /// Adds the sign-out service to use in the application.
        /// </summary>
        public BlazoradeIdBuilder AddSignOutService(Func<IServiceProvider, ISignOutService> config)
        {
            this.Services.AddScoped<ISignOutService>(sp => config.Invoke(sp));
            return this;
        }

        /// <summary>
        /// Adds the token refresher to use in the application.
        /// </summary>
        /// <typeparam name="TTokenRefresher">The type of token refresher.</typeparam>
        public BlazoradeIdBuilder AddTokenRefresher<TTokenRefresher>() where TTokenRefresher : class, ITokenRefresher
        {
            this.Services.AddScoped<ITokenRefresher, TTokenRefresher>();
            return this;
        }

        /// <summary>
        /// Adds the token refresher to use in the application.
        /// </summary>
        public BlazoradeIdBuilder AddTokenRefresher(Func<IServiceProvider, ITokenRefresher> config)
        {
            this.Services.AddScoped<ITokenRefresher>(sp => config.Invoke(sp));
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

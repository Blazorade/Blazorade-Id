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
        internal BlazoradeIdBuilder(IServiceCollection services)
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
        /// Adds authority configuration to the application.
        /// </summary>
        public BlazoradeIdBuilder AddAuthority(Action<IConfiguration, AuthorityOptions> config)
        {
            this.Services.AddOptions<AuthorityOptions>()
                .Configure<IConfiguration>((o, sp) =>
                {
                    config.Invoke(sp, o);
                });
            return this;
        }

        /// <summary>
        /// Adds the navigator service implementation to use in your application.
        /// </summary>
        public BlazoradeIdBuilder AddNavigator<TNavigator>() where TNavigator : class, INavigator
        {
            this.Services.AddScoped<INavigator, TNavigator>();
            return this;
        }

        /// <summary>
        /// Adds the storge implementations to the application's service collection.
        /// </summary>
        public BlazoradeIdBuilder AddStorage<TSessionStorage, TPersistentStorage>() where TSessionStorage : class, ISessionStorage where TPersistentStorage : class, IPersistentStorage
        {
            this.Services
                .AddScoped<StorageFacade>()
                .AddScoped<ISessionStorage, TSessionStorage>()
                .AddScoped<IPersistentStorage, TPersistentStorage>();

            return this;
        }
    }
}

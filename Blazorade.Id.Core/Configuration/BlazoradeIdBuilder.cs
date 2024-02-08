using Blazorade.Id.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Configuration
{
    public class BlazoradeIdBuilder
    {
        public BlazoradeIdBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public IServiceCollection Services { get; }

        public BlazoradeIdBuilder AddAuthority(Action<IServiceProvider, AuthorityOptions> config)
        {
            this.Services.AddOptions<AuthorityOptions>()
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                });
            return this;
        }

        public BlazoradeIdBuilder AddAuthority(string key, Action<IServiceProvider, AuthorityOptions> config)
        {
            this.Services.AddOptions<AuthorityOptions>(key)
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                });
            return this;
        }

        public BlazoradeIdBuilder AddNavigator<TNavigator>() where TNavigator : class, INavigator
        {
            this.Services.AddScoped<INavigator, TNavigator>();
            return this;
        }

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

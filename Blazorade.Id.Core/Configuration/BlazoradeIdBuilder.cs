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

        public BlazoradeIdBuilder AddOptions(Action<IServiceProvider, AuthenticationOptions> config)
        {
            this.Services.AddOptions<AuthenticationOptions>()
                .Configure<IServiceProvider>((o, sp) =>
                {
                    config.Invoke(sp, o);
                });
            return this;
        }

        public BlazoradeIdBuilder AddOptions(string key, Action<IServiceProvider, AuthenticationOptions> config)
        {
            this.Services.AddOptions<AuthenticationOptions>(key)
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
                .AddScoped<ISessionStorage, TSessionStorage>()
                .AddScoped<IPersistentStorage, TPersistentStorage>();

            return this;
        }
    }
}

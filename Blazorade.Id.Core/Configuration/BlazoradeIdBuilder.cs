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
    }
}

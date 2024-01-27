using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Microsoft.Extensions.DependencyInjection;
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

        public static BlazoradeIdBuilder AddBlazoradeId(this IServiceCollection services)
        {
            services
                .AddScoped<EndpointService>()
                .AddScoped<TokenService>()
                .AddHttpClient();

            return new BlazoradeIdBuilder(services);
        }

    }
}

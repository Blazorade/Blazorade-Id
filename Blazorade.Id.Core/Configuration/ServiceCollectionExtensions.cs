using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds Blazorade ID services to the service collection of your application.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>Returns a builder class that you use to further configure your application.</returns>
        public static BlazoradeIdBuilder AddBlazoradeId(this IServiceCollection services)
        {
            services
                .AddScoped<EndpointService>()
                .AddScoped<SerializationService>()
                .AddScoped<CodeChallengeService>()
                .AddScoped<TokenService>()
                .AddHttpClient()

                .AddOptions<JsonSerializerOptions>()
                .Configure(opt =>
                {
                    opt.PropertyNameCaseInsensitive = true;
                    opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
                ;

            return new BlazoradeIdBuilder(services);
        }

    }
}

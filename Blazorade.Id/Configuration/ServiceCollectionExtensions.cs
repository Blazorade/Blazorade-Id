using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Core.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static BlazoradeIdBuilder AddBlazoradeIdServerApplication(this IServiceCollection services)
        {
            return services.AddBlazoradeIdWasmApplication();
        }

        public static BlazoradeIdBuilder AddBlazoradeIdWasmApplication(this IServiceCollection services)
        {
            return services
                .AddCascadingAuthenticationState()
                .AddBlazoradeId();
        }
    }
}

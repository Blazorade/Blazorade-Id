using Blazorade.Id.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    internal static class ServiceExtensions
    {

        public static TestTokenRefresher GetTokenRefresher(this IServiceProvider provider)
        {
            return (TestTokenRefresher)provider.GetRequiredService<ITokenRefresher>();
        }

        public static TestRedirectUriProvider GetRedirectUriProvider(this IServiceProvider provider)
        {
            return (TestRedirectUriProvider)provider.GetRequiredService<IRedirectUriProvider>();
        }

        public static TestCodeProcessor GetAuthCodeProcessor(this IServiceProvider provider)
        {
            return (TestCodeProcessor)provider.GetRequiredService<IAuthCodeProcessor>();
        }

        public static TestCodeProvider GetAuthCodeProvider(this IServiceProvider provider)
        {
            return (TestCodeProvider)provider.GetRequiredService<IAuthCodeProvider>();
        }

    }
}

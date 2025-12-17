using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    internal static class ServiceRegistrations
    {

        public static IServiceProvider GetServiceProvider()
        {
            return new BlazoradeIdBuilder(new ServiceCollection())
                .AddAuthCodeProcessor<TestCodeProcessor>()
                .AddAuthCodeProvider<TestCodeProvider>()
                .AddRedirectUriProvider<TestRedirectUriProvider>()
                .AddTokenRefresher<TestTokenRefresher>()
                .Services

                .BuildServiceProvider();
        }
    }
}

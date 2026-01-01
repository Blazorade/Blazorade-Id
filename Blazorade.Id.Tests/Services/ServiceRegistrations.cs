using Blazorade.Id.Configuration;
using Blazorade.Id.Services;
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
            return new BlazoradeIdBuilder(new ServiceCollection().AddBlazoradeIdSharedServices())
                .AddAuthorizationCodeProcessor<TestCodeProcessor>()
                .AddAuthorizationCodeProvider<TestCodeProvider>()
                .AddRedirectUriProvider<TestRedirectUriProvider>()
                .AddTokenRefresher<TestTokenRefresher>()
                .AddRefreshTokenStore<InMemoryRefreshTokenStore>()

                .AddAuthority((sp, options) =>
                {
                    options.Scope = "openid profile email User.Read urn:blazorade:id/user_impersonation";
                })
                .Services

                .BuildServiceProvider();
        }
    }
}

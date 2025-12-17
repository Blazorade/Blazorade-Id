using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Blazorade.Id.Tests.Services;
using Microsoft.Testing.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests
{
    [TestClass]
    public class TokenHandlingTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        private IServiceProvider Provider = null!;
        private ITokenService TokenService = null!;
        [TestInitialize]
        public void TestInitialize()
        {
            this.Provider = ServiceRegistrations.GetServiceProvider();
            this.TokenService = this.Provider.GetRequiredService<ITokenService>();
        }


        [TestMethod]
        public async Task GetAccessToken001()
        {
            var at = await this.TokenService.GetAccessTokenAsync(new GetTokenOptions { Scopes = new[] { "openid", "profile", "email", "urn:blazorade:id/user_impersonation" } });
            Assert.IsNotNull(at);
        }
    }
}

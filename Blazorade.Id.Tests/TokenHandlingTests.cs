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
        private TestTokenRefresher TokenRefresher = null!;
        private TestCodeProcessor CodeProcessor = null!;
        private TestCodeProvider CodeProvider = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            this.Provider = ServiceRegistrations.GetServiceProvider();
            this.TokenService = this.Provider.GetRequiredService<ITokenService>();
            this.TokenRefresher = this.Provider.GetTokenRefresher();
            this.CodeProcessor = this.Provider.GetAuthCodeProcessor();
            this.CodeProvider = this.Provider.GetAuthCodeProvider();
        }


        [TestMethod]
        public async Task GetAccessTokens001()
        {
            this.TokenRefresher.Expiration = DateTimeOffset.UtcNow.AddHours(1);
            var ats = await this.TokenService.GetAccessTokenAsync(new GetTokenOptions { Scopes = new[] { "openid", "profile", "email", "urn:blazorade:id/user_impersonation" } });
            Assert.IsNotNull(ats);
            Assert.HasCount(2, ats);
        }

        [TestMethod]
        public async Task GetAccessTokens002()
        {
            this.TokenRefresher.Expiration = DateTimeOffset.UtcNow.AddHours(1);
            this.TokenRefresher.DisableRefresh = true; // Simulate non-existent refresh token.

            var ats = await this.TokenService.GetAccessTokenAsync();
            Assert.IsNotNull(ats);
            Assert.HasCount(1, ats);
        }
    }
}

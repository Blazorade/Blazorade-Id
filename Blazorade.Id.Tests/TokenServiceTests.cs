using Blazorade.Id.Core;
using Blazorade.Id.Core.Configuration;
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
    public class TokenServiceTests
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
        private ITokenStore TokenStore = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            this.Provider = ServiceRegistrations.GetServiceProvider();
            this.TokenService = this.Provider.GetRequiredService<ITokenService>();
            this.TokenRefresher = this.Provider.GetTokenRefresher();
            this.CodeProcessor = this.Provider.GetAuthCodeProcessor();
            this.CodeProvider = this.Provider.GetAuthCodeProvider();
            this.TokenStore = this.Provider.GetRequiredService<ITokenStore>();
        }


        [TestMethod]
        public async Task GetAccessTokens001()
        {
            await this.TokenStore.SetRefreshTokenAsync("refresh-token");
            this.TokenRefresher.Expiration = DateTimeOffset.UtcNow.AddHours(1);
            var ats = await this.TokenService.GetAccessTokensAsync(new GetTokenOptions { Scopes = new[] { "openid", "profile", "email", "urn:blazorade:id/user_impersonation" } });
            Assert.IsNotNull(ats);
            Assert.HasCount(2, ats);

            var token = ats.GetTokenByScope("urn:blazorade:id/user_impersonation");
            Assert.IsNotNull(token);
            Assert.IsTrue(token.GetExpirationTimeUtc() > DateTimeOffset.UtcNow);
        }

        [TestMethod]
        public async Task GetAccessTokens002()
        {
            this.TokenRefresher.Expiration = DateTimeOffset.UtcNow.AddHours(1);
            this.CodeProcessor.ScopesToProcess = AuthorityOptions.DefaultScope.Split(' ');

            var ats = await this.TokenService.GetAccessTokensAsync();
            Assert.IsNotNull(ats);
            Assert.HasCount(1, ats);

            var token = ats.GetTokenByScope(AuthorityOptions.DefaultScope);
            Assert.IsNotNull(token);
            var scopes = token.GetScopes();
            Assert.Contains("openid", scopes);
            Assert.Contains("profile", scopes);
            Assert.Contains("email", scopes);
        }

        [TestMethod]
        public async Task GetAccessTokens003()
        {
            this.TokenRefresher.Expiration = DateTimeOffset.UtcNow.AddHours(-1);
            this.TokenRefresher.RefreshToken = new TokenContainer("preliminary-refresh-token");
            await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions { Scopes = ["openid"] });

            this.TokenRefresher.RefreshToken = null;
            await this.TokenStore.SetRefreshTokenAsync(null);
            this.CodeProcessor.ScopesToProcess = ["openid"];

            var ats = await this.TokenService.GetAccessTokensAsync();
            Assert.HasCount(1, ats);

            var token = ats.GetTokenByScope("openid");

            Assert.IsNotNull(token);
            Assert.IsNull(token.GetExpirationTimeUtc(), "A new token must have been generated without expiration date.");
            var scopes = token.GetScopes();
            Assert.HasCount(1, scopes);
            Assert.Contains("openid", scopes);
        }

        [TestMethod]
        public async Task GetAccessTokens004()
        {

        }
    }
}

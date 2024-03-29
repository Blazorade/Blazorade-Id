using Blazorade.Id;
using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Blazorade.Id.Services;

namespace Blazorade.Id.Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public async Task LoadEndpoint01()
        {
            var svc = new EndpointService(null);
            var endpoint = await svc.GetAuthorizationEndpointAsync(new AuthorityOptions
            {
                MetadataUri = "https://login.microsoftonline.com/blazorade.com/v2.0/.well-known/openid-configuration"
            });

            Assert.AreEqual("https://login.microsoftonline.com/6cdd6461-afcf-464d-a8e9-fd4f695b5c2d/oauth2/v2.0/authorize", endpoint);
        }

        [TestMethod]
        public async Task LoadEndpoint02()
        {
            var svc = new EndpointService(null);
            var endpoint = await svc.GetTokenEndpointAsync(new AuthorityOptions
            {
                MetadataUri = "https://login.microsoftonline.com/blazorade.com/v2.0/.well-known/openid-configuration"
            });

            Assert.AreEqual("https://login.microsoftonline.com/6cdd6461-afcf-464d-a8e9-fd4f695b5c2d/oauth2/v2.0/token", endpoint);
        }

        [TestMethod]
        public async Task LoadEndpoint03()
        {
            var svc = new EndpointService(null);
            var endpoint = await svc.GetAuthorizationEndpointAsync(new AuthorityOptions
            {
                AuthorizationEndpoint = "https://site.com/foo/auth"
            });

            Assert.AreEqual("https://site.com/foo/auth", endpoint);
        }

        [TestMethod]
        public async Task LoadEndpoint04()
        {
            var svc = new EndpointService(null);
            var endpoint = await svc.GetTokenEndpointAsync(new AuthorityOptions
            {
                TokenEndpoint = "https://site.com/foo/token"
            });

            Assert.AreEqual("https://site.com/foo/token", endpoint);
        }

        [TestMethod]
        public async Task LoadEndpoint05()
        {
            var svc = new EndpointService(null);
            var options = new AuthorityOptions
            {
                MetadataUri = "https://blazoradeapps.b2clogin.com/blazoradeapps.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_unit-test"
            };

            var authEndpoint = await svc.GetAuthorizationEndpointAsync(options);
            var tokenEndpoint = await svc.GetTokenEndpointAsync(options);

            Assert.AreEqual("https://blazoradeapps.b2clogin.com/blazoradeapps.onmicrosoft.com/oauth2/v2.0/authorize?p=b2c_1_unit-test", authEndpoint);
            Assert.AreEqual("https://blazoradeapps.b2clogin.com/blazoradeapps.onmicrosoft.com/oauth2/v2.0/token?p=b2c_1_unit-test", tokenEndpoint);
        }
    }
}
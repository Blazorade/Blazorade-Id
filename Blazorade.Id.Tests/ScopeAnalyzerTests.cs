using Blazorade.Id.Services;
using Blazorade.Id.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests
{
    [TestClass]
    public class ScopeAnalyzerTests
    {
        private IServiceProvider Provider = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            this.Provider = ServiceRegistrations.GetServiceProvider();
        }


        [TestMethod]
        public async Task AnalyzeScopes01()
        {
            var analyzer = this.Provider.GetRequiredService<IScopeAnalyzer>();
            var analyzed = await analyzer.AnalyzeScopesAsync(new[] 
            { 
                "openid", 
                "profile", 
                "email",
                "User.Read",
                "Calendar.Read",
                "urn:blazorade:id/user_impersonation"
            });

            Assert.HasCount(2, analyzed);
            var scps = analyzed["urn:blazorade:id"];

            Assert.IsNotNull(scps);
            Assert.HasCount(1, scps);
            var scp = scps.Single();

            Assert.AreEqual("urn:blazorade:id/user_impersonation", scp.Value);
        }

        [TestMethod]
        public async Task AnalyzeScopes02()
        {
            string[] source = ["api://foo-bar/stuff.do", "https://api.mycompany.com/read"];
            var analyzer = this.Provider.GetRequiredService<IScopeAnalyzer>();
            var analyzed = await analyzer.AnalyzeScopesAsync(source);

            Assert.HasCount(2, analyzed);
            foreach(var key in analyzed.Keys)
            {
                var scopes = analyzed[key];
                Assert.HasCount(1, scopes);
                source.Contains(scopes.Single().Value);
                Assert.StartsWith(key, scopes.Single().Value);
            }
        }
    }
}

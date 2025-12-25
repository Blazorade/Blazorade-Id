using Blazorade.Id.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests
{
    [TestClass]
    public class ScopeSorterTests
    {

        [TestMethod]
        public async Task SortScopes01()
        {
            var sorter = new ScopeSorter();
            var sorted = await sorter.SortScopesAsync(new[] 
            { 
                "openid", 
                "profile", 
                "email",
                "User.Read",
                "Calendar.Read",
                "urn:blazorade:id/user_impersonation"
            });

            Assert.HasCount(2, sorted);
            var scps = sorted["urn:blazorade:id"];

            Assert.IsNotNull(scps);
            Assert.HasCount(1, scps);
            var scp = scps.Single();

            Assert.AreEqual("urn:blazorade:id/user_impersonation", scp.Value);
        }

        [TestMethod]
        public async Task SortScopes02()
        {
            string[] source = ["api://foo-bar/stuff.do", "https://api.mycompany.com/read"];
            var sorter = new ScopeSorter();
            var sorted = await sorter.SortScopesAsync(source);

            Assert.HasCount(2, sorted);
            foreach(var key in sorted.Keys)
            {
                var scopes = sorted[key];
                Assert.HasCount(1, scopes);
                source.Contains(scopes.Single().Value);
                Assert.StartsWith(key, scopes.Single().Value);
            }
        }
    }
}

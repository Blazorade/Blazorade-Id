using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
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
        public void SortScopes01()
        {
            var sorter = new ScopeSorter();
            var sorted = sorter.SortScopes(new[] 
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

            Assert.AreEqual("urn:blazorade:id", scp.ResourceId);
            Assert.AreEqual("user_impersonation", scp.Value);
        }

        [TestMethod]
        public void SortScopes02()
        {
            var sorter = new ScopeSorter();
            var sorted = sorter.SortScopes(new[] {
                "api://foo-bar/stuff.do",
                "https://api.mycompany.com/read"
            });

            Assert.HasCount(2, sorted);
            foreach(var key in sorted.Keys)
            {
                var scopes = sorted[key] ?? [];
                Assert.HasCount(1, scopes);
                Assert.IsTrue(scopes.All(x => x.ResourceId == key));
            }
        }
    }
}

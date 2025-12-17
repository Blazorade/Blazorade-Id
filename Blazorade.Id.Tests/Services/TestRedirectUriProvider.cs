using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestRedirectUriProvider : IRedirectUriProvider
    {
        public Uri GetRedirectUri()
        {
            return new Uri("https://localhost/blazorade-id/oauth-callback");
        }
    }
}

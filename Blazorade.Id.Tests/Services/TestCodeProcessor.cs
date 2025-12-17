using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestCodeProcessor : IAuthCodeProcessor
    {
        public TestCodeProcessor(ITokenStore tokenStore)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        }

        private readonly ITokenStore TokenStore;


        public async Task<bool> ProcessAuthorizationCodeAsync(string code)
        {
            return await Task.FromResult(true);
        }
    }
}

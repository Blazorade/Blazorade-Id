using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestCodeProvider : IAuthCodeProvider
    {
        public async Task<string?> GetAuthorizationCodeAsync(GetTokenOptions options)
        {
            var code = $"{Guid.NewGuid()}";
            return await Task.FromResult(code);
        }
    }
}

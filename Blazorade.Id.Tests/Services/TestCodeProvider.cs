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
        public string? AuthCode { get; set; }

        public async Task<string?> GetAuthorizationCodeAsync(GetTokenOptions options)
        {
            var code = this.AuthCode ?? $"{Guid.NewGuid()}";
            return await Task.FromResult(code);
        }
    }
}

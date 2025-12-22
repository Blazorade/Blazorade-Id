using Blazorade.Id.Model;
using Blazorade.Id.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestCodeProvider : IAuthorizationCodeProvider
    {
        public string? AuthCode { get; set; }

        public async Task<AuthorizationCodeResult> GetAuthorizationCodeAsync(GetTokenOptions options)
        {
            var result = new AuthorizationCodeResult
            {
                Code = this.AuthCode ?? $"{Guid.NewGuid()}"
            };
            return await Task.FromResult(result);
        }
    }
}

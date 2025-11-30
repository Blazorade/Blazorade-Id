using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public interface IAuthCodeProvider
    {
        Task<string?> GetAuthorizationCodeAsync(GetTokenOptions options);
    }
}

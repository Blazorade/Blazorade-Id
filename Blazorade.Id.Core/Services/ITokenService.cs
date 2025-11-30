using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public interface ITokenService
    {
        Task<JwtSecurityToken?> GetAccessTokenAsync(GetTokenOptions? options = null);

        Task<JwtSecurityToken?> GetIdentityTokenAsync(GetTokenOptions? options = null);
    }
}

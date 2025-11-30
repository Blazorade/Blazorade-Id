using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class AuthCodeProcessor : IAuthCodeProcessor
    {
        public async Task<bool> ProcessAuthorizationCodeAsync(string code)
        {

            return false;
        }
    }
}

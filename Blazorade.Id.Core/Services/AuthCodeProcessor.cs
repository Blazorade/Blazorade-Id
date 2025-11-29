using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class AuthCodeProcessor : IAuthCodeProcessor
    {
        public Task<bool> ProcessAuthorizationCodeAsync(string code)
        {
            throw new NotImplementedException();
        }
    }
}

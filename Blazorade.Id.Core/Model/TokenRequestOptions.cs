using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    public class TokenRequestOptions : TokenServiceOptionsBase
    {

        public string? AuthorizationToken { get;set; }

        Prompt? Prompt { get;set; }
    }
}

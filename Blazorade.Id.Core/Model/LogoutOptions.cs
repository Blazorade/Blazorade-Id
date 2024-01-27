using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    public class LogoutOptions : TokenServiceOptionsBase
    {

        public string? PostLogoutRedirectUri { get; set; }

    }
}

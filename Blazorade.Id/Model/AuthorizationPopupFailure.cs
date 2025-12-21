using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Core.Model;

namespace Blazorade.Id.Model
{
    internal class AuthorizationPopupFailure
    {
        public string Error { get; set; } = string.Empty;

        public AuthorizationCodeFailure Reason { get; set; }
    }
}

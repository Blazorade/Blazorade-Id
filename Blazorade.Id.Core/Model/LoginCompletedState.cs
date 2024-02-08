using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    public class LoginCompletedState
    {

        public bool IsSuccess { get; set; }

        public TokenError? Error { get; set; }

        public string? AuthorityKey { get; set; }

        public string? Username {  get; set; }

        public string? DisplayName {  get; set; }

    }
}

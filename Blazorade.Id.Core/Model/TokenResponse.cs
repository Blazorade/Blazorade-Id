using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    public class TokenResponse
    {

        public TokenResponse(TokenSet set)
        {
            this.Tokens = set;
            this.Error = null;
            this.IsSuccess = true;
        }

        public TokenResponse(TokenError error)
        {
            this.Error = error;
            this.Tokens = null;
            this.IsSuccess = false;
        }

        public bool IsSuccess { get; private set; }

        public TokenSet? Tokens { get; private set; }

        public TokenError? Error { get; private set; }

    }
}

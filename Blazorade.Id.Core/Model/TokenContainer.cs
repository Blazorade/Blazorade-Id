using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    public class TokenContainer
    {
        public TokenContainer() { }

        public TokenContainer(string? token, DateTime? expires)
        {
            this.Token = token;
            this.Expires = expires;
        }

        public string? Token {  get; set; }

        public DateTime? Expires { get; set; }

        public JwtSecurityToken? ParseToken()
        {
            if(this.Token?.Length > 0)
            {
                return new JwtSecurityToken(this.Token);
            }

            return null;
        }
    }
}

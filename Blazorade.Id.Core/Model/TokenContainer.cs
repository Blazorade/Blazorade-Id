using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// A container for a token.
    /// </summary>
    public class TokenContainer
    {

        /// <summary>
        /// The default constructor for the <see cref="TokenContainer"/> class.
        /// </summary>
        public TokenContainer() { }

        /// <summary>
        /// Creates the container specifying the raw contents of the container and its expiration date.
        /// </summary>
        /// <param name="token">The raw encoded representation of the token.</param>
        /// <param name="expires">The expiration timestamp, in UTC.</param>
        public TokenContainer(string? token, DateTime? expires = null)
        {
            this.Token = token;
            this.Expires = expires;
        }

        /// <summary>
        /// Creates a container from the given <see cref="JwtSecurityToken"/> instance.
        /// </summary>
        public TokenContainer(JwtSecurityToken? token) : this(token?.RawData, token?.GetExpirationTimeUtc())
        {
        }

        /// <summary>
        /// The raw encoded representation of the token.
        /// </summary>
        public string? Token {  get; set; }

        /// <summary>
        /// The date and time, in UTC, when the token expires.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Parses the encoded token into a <see cref="JwtSecurityToken"/> object.
        /// </summary>
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

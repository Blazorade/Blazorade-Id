using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Services;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Options for signing users in with the <see cref="IAuthenticationService"/>.
    /// </summary>
    public class SignInOptions
    {
        /// <summary>
        /// Prepopulates the username during sign-in and skips the account picker when possible. It suggests which user is expected but does not authenticate them.
        /// </summary>
        public string? LoginHint { get; set; }

        /// <summary>
        /// Guides the IdP's home-realm discovery by indicating the user’s domain or identity provider. It speeds up routing but does not enforce tenant selection.
        /// </summary>
        public string? DomainHint { get; set; }

        /// <summary>
        /// How the user should be prompted during authentication.
        /// </summary>
        public Prompt? Prompt { get; set; }

        /// <summary>
        /// A set of scopes to request when acquiring the token.
        /// </summary>
        public IEnumerable<string>? Scopes { get; set; }



        /// <summary>
        /// Returns a <see cref="GetTokenOptions"/> instance based on the current <see cref="SignInOptions"/>.
        /// </summary>
        public GetTokenOptions ToGetTokenOptions()
        {
            return new GetTokenOptions
            {
                DomainHint = this.DomainHint,
                LoginHint = this.LoginHint,
                Prompt = this.Prompt,
                Scopes = this.Scopes
            };
        }
    }
}

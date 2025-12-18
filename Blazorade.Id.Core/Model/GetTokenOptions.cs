using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Defines a set of options that can be used when acquiring a token.
    /// </summary>
    public class GetTokenOptions
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
        /// Returns <see langword="true"/> if the current options contains all of the specified <paramref name="scopes"/>;
        /// </summary>
        /// <param name="scopes">
        /// The scopes to match against <see cref="Scopes"/>. All of the given scopes must exist in <see cref="Scopes"/>.
        /// </param>
        public bool ContainsScopes(params string[] scopes)
        {
            var current = this.Scopes ?? [];
            return scopes.All(x => current.Contains(x));
        }
    }

}

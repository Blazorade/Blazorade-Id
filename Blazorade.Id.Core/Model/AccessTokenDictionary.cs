using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Services;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Represents a set of access tokens that are grouped by the resource they are intended for and keyed by the
    /// identifier of that resource. The dictionary is produced by an implementation of the 
    /// <see cref="Services.ITokenService"/> service interface.
    /// </summary>
    public class AccessTokenDictionary : DictionaryBase<string, JwtSecurityToken>
    {

        /// <summary>
        /// Returns the token that is intended for the given scope.
        /// </summary>
        /// <param name="scope">A string containing the scope to return the token for.</param>
        /// <remarks>
        /// Even though you can supply multiple scopes in the <paramref name="scope"/> parameter
        /// by separating them with a space, only the first scope is used to match the token. If
        /// you have acquired the access tokens using an implementation of the <see cref="ITokenService"/>
        /// service interface, the returned token will contain all the scopes that were requested for the
        /// same target resource.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The exception that is thrown if <paramref name="scope"/> is <see langword="null"/>.</exception>
        public JwtSecurityToken? GetTokenByScope(string scope)
        {
            if(null == scope) throw new ArgumentNullException(nameof(scope));
            if (scope.Contains(' '))
            {
                var arr = scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                scope = arr[0];
            }

            foreach(var token in this.Values)
            {
                if(token.GetScopes().Contains(scope))
                {
                    return token;
                }
            }

            return null;
        }
    }
}

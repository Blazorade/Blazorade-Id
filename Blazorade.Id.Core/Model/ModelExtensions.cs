using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Extension methods for types defined in the <see cref="Model"/> namespace.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Examines the scope contained in the current <see cref="TokenContainer"/> and determines
        /// whether it contains all of the specified <paramref name="scopes"/>.
        /// </summary>
        /// <param name="container">The container to examine.</param>
        /// <param name="scopes">The scopes to check. All of these scopes must be present in the container.</param>
        public static bool ContainsScopes(this TokenContainer? container, params string[] scopes)
        {
            return null != container?.Scopes && scopes.All(s => container.Scopes.Contains(s));
        }

        /// <summary>
        /// Examines the scope contained in the current <see cref="TokenContainer"/> and determines
        /// whether it contains all of the specified <paramref name="scopes"/>.
        /// </summary>
        /// <param name="container">The container to examine.</param>
        /// <param name="scopes">The scopes to check. All of these scopes must be present in the container.</param>
        public static bool ContainsScopes(this TokenContainer? container, IEnumerable<string> scopes)
        {
            return container.ContainsScopes(scopes.ToArray());
        }

        /// <summary>
        /// Returns the token from the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to get the token from.</param>
        /// <param name="scopes">The scopes that the token must contain.</param>
        /// <returns>The token if it is valid and contains the required scopes; otherwise, <c>null</c>.</returns>
        public static JwtSecurityToken? GetToken(this TokenContainer? container, IEnumerable<string>? scopes)
        {
            if (container?.Expires > DateTime.UtcNow)
            {
                var token = container.ParseToken();
                return token;
            }
            return null;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="prompt"/> requires user interaction.
        /// </summary>
        public static bool RequiresInteraction(this Prompt? prompt)
        {
            return prompt.HasValue && (prompt == Prompt.Login || prompt == Prompt.Consent || prompt == Prompt.Select_Account);
        }

    }
}

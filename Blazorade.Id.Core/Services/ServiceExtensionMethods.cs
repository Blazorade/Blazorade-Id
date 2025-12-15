using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Extenion methods for services.
    /// </summary>
    public static class ServiceExtensionMethods
    {
        /// <summary>
        /// Stores the given access token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the access token.</returns>
        public async static ValueTask<TokenContainer?> SetAccessTokenAsync(this ITokenStore store, string? token)
        {
            var jwt = new JwtSecurityToken(token);
            return await store.SetAccessTokenAsync(jwt);
        }

        /// <summary>
        /// Stores the given access token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the access token.</returns>
        public async static ValueTask<TokenContainer?> SetAccessTokenAsync(this ITokenStore store, JwtSecurityToken? token)
        {
            if(null != token)
            {
                var container = new TokenContainer(token);
                await store.SetAccessTokenAsync(container);
                return container;
            }
            return null;
        }

        /// <summary>
        /// Stores the given identity token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the identity token.</returns>
        public async static ValueTask<TokenContainer?> SetIdentityTokenAsync(this ITokenStore store, string? token)
        {
            var jwt = new JwtSecurityToken(token);
            return await store.SetIdentityTokenAsync(jwt);
        }

        /// <summary>
        /// Stores the given identity token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the identity token.</returns>
        public async static ValueTask<TokenContainer?> SetIdentityTokenAsync(this ITokenStore store, JwtSecurityToken? token)
        {
            if (null != token)
            {
                var container = new TokenContainer(token);
                await store.SetIdentityTokenAsync(container);
                return container;
            }
            return null;
        }

        /// <summary>
        /// Stores the given refresh token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the refresh token.</returns>
        public static async ValueTask<TokenContainer?> SetRefreshTokenAsync(this ITokenStore store, string token)
        {
            var container = new TokenContainer(token);
            await store.SetRefreshTokenAsync(container);
            return container;
        }
    }
}

using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Extenion methods for services.
    /// </summary>
    public static class ServiceExtensionMethods
    {

        private const string CodeVerifierKey = "codeVerifier";
        private const string LoginHintKey = "loginHint";
        private const string ScopeKey = "scope";
        private const string NonceKey = "nonce";


        /// <summary>
        /// Creates an endpoint uri builder that points at the authorization endpoint.
        /// </summary>
        public static async Task<EndpointUriBuilder> CreateAuthorizationUriBuilderAsync(this IEndpointService epService, ICodeChallengeService codeChallengeService)
        {
            var uri = await epService.GetAuthorizationEndpointAsync() ?? throw new Exception("Could not resolve URI for authorization endpoint.");
            return new EndpointUriBuilder(uri, codeChallengeService);
        }

        /// <summary>
        /// Creates an endpoint uri builder that points at the end session endpoint.
        /// </summary>
        public static async Task<EndpointUriBuilder> CreateEndSessionUriBuilderAsync(this IEndpointService epService)
        {
            var uri = await epService.GetEndSessionEndpointAsync() ?? throw new Exception("Could not resolve URI for end session endpoint.");
            return new EndpointUriBuilder(uri);
        }

        /// <summary>
        /// Creates a token endpoint uri builder.
        /// </summary>
        public static async Task<TokenEndpointUriBuilder> CreateTokenRequestBuilderAsync(this IEndpointService epService)
        {
            var uri = await epService.GetTokenEndpointAsync() ?? throw new Exception("Could not resolve URI for token endpoint");
            return new TokenEndpointUriBuilder(uri);
        }


        /// <summary>
        /// Returns the code verifier that was used when starting the current login process.
        /// </summary>
        public static async Task<string?> GetCodeVerifierAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(CodeVerifierKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the login hint that was used when starting the previous login process.
        /// </summary>
        public static async Task<string?> GetLoginHintAsync(this IPropertyStore store)
        {
            var key = PrefixKey(LoginHintKey);
            return await store.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the nonce that was used when starting the current login process.
        /// </summary>
        public static async Task<string?> GetNonceAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(NonceKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the scope that was used when starting the current login process.
        /// </summary>
        public static async Task<string?> GetScopeAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(ScopeKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Removes the code verifier that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public static async Task RemoveCodeVerifierAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(CodeVerifierKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public static async Task RemoveNonceAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(NonceKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public static async Task RemoveScopeAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(ScopeKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Stores the given access token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the access token.</returns>
        public async static Task<TokenContainer?> SetAccessTokenAsync(this ITokenStore store, string resourceId, string? token)
        {
            var jwt = new JwtSecurityToken(token);
            return await store.SetAccessTokenAsync(resourceId, jwt);
        }

        /// <summary>
        /// Stores the given access token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the access token.</returns>
        public async static Task<TokenContainer?> SetAccessTokenAsync(this ITokenStore store, string resourceId, JwtSecurityToken? token)
        {
            if(null != token)
            {
                var container = new TokenContainer(token);
                await store.SetAccessTokenAsync(resourceId, container);
                return container;
            }
            return null;
        }

        /// <summary>
        /// Sets the code verifier that was used when starting the current login process.
        /// </summary>
        public static async Task SetCodeVerifierAsync(this IPropertyStore storage, string? codeVerifier)
        {
            var key = PrefixKey(CodeVerifierKey);
            await storage.SetPropertyAsync(key, codeVerifier);
        }

        /// <summary>
        /// Stores the given identity token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the identity token.</returns>
        public async static Task<TokenContainer?> SetIdentityTokenAsync(this ITokenStore store, string? token)
        {
            var jwt = new JwtSecurityToken(token);
            return await store.SetIdentityTokenAsync(jwt);
        }

        /// <summary>
        /// Stores the given identity token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the identity token.</returns>
        public async static Task<TokenContainer?> SetIdentityTokenAsync(this ITokenStore store, JwtSecurityToken? token)
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
        /// Stores the given login hint in the property store.
        /// </summary>
        /// <remarks>
        /// Setting the login hint to a <see langword="null"/> value removes any previously stored login hint.
        /// </remarks>
        public static Task SetLoginHintAsync(this IPropertyStore store, string? loginHint)
        {
            var key = PrefixKey(LoginHintKey);
            if(null != loginHint)
            {
                return store.SetPropertyAsync(key, loginHint);
            }

            return store.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        public static async Task SetNonceAsync(this IPropertyStore storage, string? nonce)
        {
            var key = PrefixKey(NonceKey);
            await storage.SetPropertyAsync(key, nonce);
        }

        /// <summary>
        /// Stores the given refresh token in the token store.
        /// </summary>
        /// <returns>Returns the container holding the refresh token.</returns>
        public static async Task<TokenContainer?> SetRefreshTokenAsync(this ITokenStore store, string token)
        {
            var container = new TokenContainer(token);
            await store.SetRefreshTokenAsync(container);
            return container;
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public static async Task SetScopeAsync(this IPropertyStore storage, string? scope)
        {
            var key = PrefixKey(ScopeKey);
            await storage.SetPropertyAsync(key, scope);
        }



        private static string PrefixKey(string key, string? authKey = null)
        {
            return authKey?.Length > 0 ? $"blazorade.id.{authKey}.{key}" : $"blazorade.id.{key}";
        }
    }
}

using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service implementation for working with tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Creates a new instance of the token service.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is thrown if an argument is <c>null</c>.</exception>
        public TokenService(
            IHttpClientFactory httpFactory, 
            ITokenStore tokenStore,
            IPropertyStore propertyStore,
            IOptions<AuthorityOptions> authOptions, 
            EndpointService epService, 
            IOptions<JsonSerializerOptions> jsonOptions,
            IAuthCodeProvider authCodeProvider,
            IAuthCodeProcessor authCodeProcessor
        ) {
            this.HttpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.AuthOptions = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.EndpointService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.JsonOptions = jsonOptions.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.AuthCodeProvider = authCodeProvider ?? throw new ArgumentNullException(nameof(authCodeProvider));
            this.AuthCodeProcessor = authCodeProcessor ?? throw new ArgumentNullException(nameof(authCodeProcessor));
        }

        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly AuthorityOptions AuthOptions;
        private readonly EndpointService EndpointService;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly IAuthCodeProvider AuthCodeProvider;
        private readonly IAuthCodeProcessor AuthCodeProcessor;

        /// <summary>
        /// Returns the access token for the current signed in user.
        /// </summary>
        /// <param name="scopes">The scopes that are required to be present in the access token.</param>
        /// <remarks>
        /// This service will attempt to refresh the access token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<JwtSecurityToken?> GetAccessTokenAsync(GetTokenOptions? options = null)
        {
            options = options ?? new GetTokenOptions();
            options.Scopes = options.Scopes ?? $"{this.AuthOptions.Scope}".Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var token = await this.GetValidTokenAsync(this.TokenStore.GetAccessTokenAsync, options);
            return token;
        }

        /// <summary>
        /// Returns the identity token for the current signed in user.
        /// </summary>
        /// <remarks>
        /// This service will attempt to refresh the identity token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<JwtSecurityToken?> GetIdentityTokenAsync(GetTokenOptions? options = null)
        {
            options = options ?? new GetTokenOptions();
            options.Scopes = []; // Identity tokens do not have scopes.

            var token = await this.GetValidTokenAsync(this.TokenStore.GetIdentityTokenAsync, options);
            return token;
        }

        /// <summary>
        /// Processes the given authorization code. The tokens acquired with the given code are stored
        /// in the configured storage.
        /// </summary>
        /// <param name="code">The authorization code received from the authorization endpoint.</param>
        /// <param name="redirectUri">
        /// The URI that was specified when sending the user to the authorization endpoint.
        /// </param>
        /// <param name="expectedNonce">
        /// The nonce that was sent to the authorization endpoint. If this is set, then the same 
        /// nonce must be found as a nonce claim in the identity token. If not, the identity 
        /// token will be rejected.
        /// </param>
        /// <returns>A set of tokens that were acquired with the authorization code.</returns>
        /// <exception cref="ArgumentException">
        /// The exception that is thrown if <paramref name="redirectUri"/> is not an absolute URI.
        /// </exception>
        public async ValueTask<OperationResult<TokenSet>> ProcessAuthorizationCodeAsync(string code, string redirectUri, string? expectedNonce)
        {
            var uri = new Uri(redirectUri);
            if(!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The given redirect URI must be an absolute URI, and it must match the redirect URI that was specified in the login request sent to the authorization endpoint.", nameof(redirectUri));
            }

            var codeVerifier = await this.PropertyStore.GetCodeVerifierAsync();
            var scope = await this.PropertyStore.GetScopeAsync();

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
            var tokenRequest = tokenRequestBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(uri)
                .Build();

            await this.PropertyStore.RemoveScopeAsync();
            await this.PropertyStore.RemoveCodeVerifierAsync();

            return await this.ExecuteTokenEndpointRequestAsync(tokenRequest, expectedNonce);
        }

        /// <summary>
        /// Processes the given identity token and stores it in the configured storage and can be accessed
        /// with the <see cref="GetIdentityTokenAsync"/> method.
        /// </summary>
        /// <param name="idToken">The raw identity token to process.</param>
        /// <param name="expectedNonce">
        /// The nonce to expect in the identity token. If the nonce is given, the same nonce must be found in
        /// the identity token as a nonce claim. If not, the token will be rejected.
        /// </param>
        public async ValueTask<OperationResult<TokenContainer>> ProcessIdentityTokenAsync(string idToken, string? expectedNonce)
        {
            JwtSecurityToken? token = null;
            TokenContainer? container = null;
            OperationError? error = null;

            try
            {
                token = new JwtSecurityToken(idToken);
                var expires = token.GetExpirationTimeUtc();
                if(expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                expectedNonce = expectedNonce ?? await this.PropertyStore.GetNonceAsync();
                await this.PropertyStore.RemoveNonceAsync();
                var tokenNonce = token.GetNonce();

                if(tokenNonce?.Length > 0 && expectedNonce?.Length > 0 && tokenNonce != expectedNonce)
                {
                    // If the token has a nonce specified, but it does not match
                    // the one that was stored for the session, then we cannot
                    // accept the token.
                    throw new Exception("The token nonce does not match the requested nonce.");
                }

                var username = token.GetClaimValue("preferred_username");
                await this.PropertyStore.SetUsernameAsync(username);

                container = new TokenContainer(idToken, expires);
                await this.TokenStore.SetIdentityTokenAsync(container);
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            container = new TokenContainer(token: idToken, expires: null);
            return new OperationResult<TokenContainer>(value: container, error: error);
        }

        /// <summary>
        /// Processes the given access token and stores it in the configured storage. It can then be accessed
        /// using the <see cref="GetAccessTokenAsync"/> method.
        /// </summary>
        /// <param name="accessToken">The access token to process.</param>
        public async ValueTask<OperationResult<TokenContainer>> ProcessAccessTokenAsync(string accessToken)
        {
            JwtSecurityToken? token = null;
            TokenContainer? container = null;
            OperationError? error = null;

            // appid = clientId

            try
            {
                token = new JwtSecurityToken(accessToken);
                var expires = token.GetExpirationTimeUtc();
                if (expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                container = new TokenContainer(accessToken, expires);
                await this.TokenStore.SetAccessTokenAsync(container);
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            container = new TokenContainer(token: accessToken, expires: null);
            return new OperationResult<TokenContainer>(value: container, error: error);
        }



        private async Task<bool> AcquireTokensInteractiveAsync(GetTokenOptions options)
        {
            if(options.Prompt == Prompt.Login)
            {
                options.LoginHint = await this.PropertyStore.GetUsernameAsync();
            }

            var code = await this.AuthCodeProvider.GetAuthorizationCodeAsync(options);
            if(code?.Length > 0)
            {
                return await this.AuthCodeProcessor.ProcessAuthorizationCodeAsync(code);
            }

            return false;
        }

        private async ValueTask<OperationResult<TokenSet>> ExecuteTokenEndpointRequestAsync(HttpRequestMessage request, string? expectedNonce)
        {
            TokenSet? tokenSet = null;
            OperationError? error = null;

            var now = DateTime.UtcNow;
            var client = this.HttpClientFactory.CreateClient();
            try
            {
                using(var response = await client.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        tokenSet = JsonSerializer.Deserialize<TokenSet>(content, options: this.JsonOptions);
                        if (null != tokenSet)
                        {
                            tokenSet.ExpiresAtUtc = now.AddSeconds(tokenSet.ExpiresIn);
                        }
                    }
                    else
                    {
                        error = new OperationError { Code = response.StatusCode.ToString(), Description = content };
                    }
                }
            }
            catch(Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            if (tokenSet?.RefreshToken?.Length > 0)
            {
                // We don't set the expiration for the refresh token, because it does not expire at the same time with 
                // the other tokens.
                await this.TokenStore.SetRefreshTokenAsync(new TokenContainer(tokenSet.RefreshToken, null));
            }

            if (tokenSet?.IdentityToken?.Length > 0)
            {
                await this.ProcessIdentityTokenAsync(tokenSet.IdentityToken, expectedNonce);
            }

            if (tokenSet?.AccessToken?.Length > 0)
            {
                await this.ProcessAccessTokenAsync(tokenSet.AccessToken);
            }

            return new OperationResult<TokenSet>(tokenSet, error);
        }

        /// <summary>
        /// Uses the given <paramref name="tokenGetter"/> to get a valid token. If a valid token is not found,
        /// then the service will attempt to refresh the tokens using a refresh token, if available. If refresh
        /// is successful, the valid token is returned using the same <paramref name="tokenGetter"/>.
        /// </summary>
        /// <param name="tokenGetter">A delegate that is used to get a token from storage.</param>
        /// <param name="scopes">The scopes that the returned token must contain. An empty array will ignore the scopes.</param>
        private async ValueTask<JwtSecurityToken?> GetValidTokenAsync(Func<ValueTask<TokenContainer?>> tokenGetter, GetTokenOptions options)
        {
            JwtSecurityToken? token = null!;
            TokenContainer? tokenContainer = null;

            if (!options.Prompt.RequiresInteraction())
            {
                tokenContainer = await tokenGetter();
                token = tokenContainer?.GetToken(options.Scopes);
            }

            if (null == token)
            {
                if (!options.Prompt.RequiresInteraction())
                {
                    // Try to refresh tokens silently first.
                    if (await this.RefreshTokensAsync())
                    {
                        tokenContainer = await tokenGetter();
                        token = tokenContainer?.GetToken(options.Scopes);
                    }
                }

                if (null == token)
                {
                    var result = await this.AcquireTokensInteractiveAsync(options);
                    if (result)
                    {
                        tokenContainer = await tokenGetter();
                        token = tokenContainer?.GetToken(options.Scopes);
                    }
                }
            }

            return token;
        }

        private async ValueTask<bool> RefreshTokensAsync()
        {
            var refreshToken = await this.TokenStore.GetRefreshTokenAsync();
            if(refreshToken?.Token?.Length > 0)
            {
                var requestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
                var request = requestBuilder
                    .WithClientId(this.AuthOptions.ClientId)
                    .WithRefreshToken(refreshToken.Token)
                    .WithRedirectUri(this.AuthOptions.RedirectUri)
                    .Build();

                var result = await this.ExecuteTokenEndpointRequestAsync(request, null);
                return result.IsSuccess;
            }

            return false;
        }

    }
}

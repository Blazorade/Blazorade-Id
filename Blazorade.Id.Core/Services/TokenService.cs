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
    public class TokenService
    {
        /// <summary>
        /// Creates a new instance of the token service.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is thrown if an argument is <c>null</c>.</exception>
        public TokenService(IHttpClientFactory httpFactory, StorageFacade storage, IOptions<AuthorityOptions> authOptions, EndpointService epService, IOptions<JsonSerializerOptions> jsonOptions, INavigator navigator)
        {
            this.HttpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.StorageFacade = storage ?? throw new ArgumentNullException(nameof(storage));
            this.AuthOptions = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.EndpointService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.JsonOptions = jsonOptions.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        private readonly IHttpClientFactory HttpClientFactory;
        private readonly StorageFacade StorageFacade;
        private readonly AuthorityOptions AuthOptions;
        private readonly EndpointService EndpointService;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly INavigator Navigator;

        /// <summary>
        /// Returns the access token for the current signed in user.
        /// </summary>
        /// <remarks>
        /// This service will attempt to refresh the access token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async ValueTask<JwtSecurityToken?> GetAccessTokenAsync()
        {
            return await this.GetValidTokenAsync(this.StorageFacade.GetAccessTokenAsync);
        }

        /// <summary>
        /// Returns the identity token for the current signed in user.
        /// </summary>
        /// <remarks>
        /// This service will attempt to refresh the identity token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async ValueTask<JwtSecurityToken?> GetIdentityTokenAsync()
        {
            return await this.GetValidTokenAsync(this.StorageFacade.GetIdentityTokenAsync);
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

            var codeVerifier = await this.StorageFacade.GetCodeVerifierAsync();
            var scope = await this.StorageFacade.GetScopeAsync();

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
            var tokenRequest = tokenRequestBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(uri)
                .Build();

            await this.StorageFacade.RemoveScopeAsync();
            await this.StorageFacade.RemoveCodeVerifierAsync();

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
                var expires = this.GetExpirationTimeUtc(token);
                if(expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                expectedNonce = expectedNonce ?? await this.StorageFacade.GetNonceAsync();
                await this.StorageFacade.RemoveNonceAsync();
                var tokenNonce = this.GetNonce(token);

                if(tokenNonce?.Length > 0 && expectedNonce?.Length > 0 && tokenNonce != expectedNonce)
                {
                    // If the token has a nonce specified, but it does not match
                    // the one that was stored for the session, then we cannot
                    // accept the token.
                    throw new Exception("The token nonce does not match the requested nonce.");
                }

                var username = this.GetClaimValue(token, "preferred_username");
                await this.StorageFacade.SetUsernameAsync(username);

                container = new TokenContainer(idToken, expires);
                await this.StorageFacade.SetIdentityTokenAsync(container);
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
                var expires = this.GetExpirationTimeUtc(token);
                if (expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                container = new TokenContainer(accessToken, expires);
                await this.StorageFacade.SetAccessTokenAsync(container);
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            container = new TokenContainer(token: accessToken, expires: null);
            return new OperationResult<TokenContainer>(value: container, error: error);
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
                await this.StorageFacade.SetRefreshTokenAsync(new TokenContainer(tokenSet.RefreshToken, null));
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

        private DateTime? GetExpirationTimeUtc(JwtSecurityToken token)
        {
            var exp = this.GetClaimValue(token, "exp");
            if(long.TryParse(exp, out long l))
            {
                var expires = DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime;
                return expires;
            }

            return null;
        }

        private string? GetNonce(JwtSecurityToken token)
        {
            return this.GetClaimValue(token, "nonce");
        }

        private string? GetClaimValue(JwtSecurityToken token, string claimType)
        {
            return token.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value;
        }

        private async ValueTask<JwtSecurityToken?> GetValidTokenAsync(Func<ValueTask<TokenContainer?>> tokenGetter)
        {
            JwtSecurityToken? token = null!;
            Func<ValueTask<JwtSecurityToken?>> storageAccessor = async () =>
            {
                var container = await tokenGetter();
                if (container?.Expires > DateTime.UtcNow && container?.Token?.Length > 0)
                {
                    return new JwtSecurityToken(container.Token);
                }

                return null;
            };

            token = await storageAccessor();
            if(null == token && await this.RefreshTokensAsync())
            {
                token = await storageAccessor();
            }

            return token;
        }

        private async ValueTask<bool> RefreshTokensAsync()
        {
            var refreshToken = await this.StorageFacade.GetRefreshTokenAsync();
            if(refreshToken?.Token?.Length > 0)
            {
                var requestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
                var request = requestBuilder
                    .WithClientId(this.AuthOptions.ClientId)
                    .WithRefreshToken(refreshToken.Token)
                    .WithRedirectUri(this.Navigator.CurrentUri)
                    .Build();

                var result = await this.ExecuteTokenEndpointRequestAsync(request, null);
                return result.IsSuccess;
            }

            return false;
        }

    }
}

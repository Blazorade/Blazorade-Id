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
    public class TokenService
    {
        public TokenService(IHttpClientFactory httpFactory, StorageFacade storage, IOptionsFactory<AuthorityOptions> authOptions, EndpointService epService, IOptions<JsonSerializerOptions> jsonOptions, INavigator navigator)
        {
            this.HttpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.StorageFacade = storage ?? throw new ArgumentNullException(nameof(storage));
            this.AuthOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
            this.EndpointService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.JsonOptions = jsonOptions.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        private readonly IHttpClientFactory HttpClientFactory;
        private readonly StorageFacade StorageFacade;
        private readonly IOptionsFactory<AuthorityOptions> AuthOptions;
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

        public async ValueTask<OperationResult<TokenSet>> ProcessAuthorizationCodeAsync(string code, string redirectUri, string? expectedNonce, string? authorityKey)
        {
            var uri = new Uri(redirectUri);
            if(!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The given redirect URI must be an absolute URI, and it must match the redirect URI that was specified in the login request sent to the authorization endpoint.", nameof(redirectUri));
            }

            var codeVerifier = await this.StorageFacade.GetCodeVerifierAsync();
            var scope = await this.StorageFacade.GetScopeAsync();
            var options = this.AuthOptions.Create(authorityKey ?? "");

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync(options);
            var tokenRequest = tokenRequestBuilder
                .WithClientId(options.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(uri)
                .Build();

            await this.StorageFacade.RemoveScopeAsync();
            await this.StorageFacade.RemoveCodeVerifierAsync();

            return await this.ExecuteTokenEndpointRequestAsync(tokenRequest, expectedNonce, authorityKey);
        }

        public async ValueTask<OperationResult<TokenContainer>> ProcessIdentityTokenAsync(string idToken, string? expectedNonce, string? authorityKey)
        {
            JwtSecurityToken? token = null;
            TokenContainer? container = null;
            OperationError? error = null;

            // aud = clientId

            try
            {
                token = new JwtSecurityToken(idToken);
                var expires = this.GetExpirationTimeUtc(token);
                if(expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                var tokenNonce = this.GetNonce(token);
                await this.StorageFacade.RemoveNonceAsync();

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

        public async ValueTask<OperationResult<TokenContainer>> ProcessAccessTokenAsync(string accessToken, string? authorityKey)
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



        private async ValueTask<OperationResult<TokenSet>> ExecuteTokenEndpointRequestAsync(HttpRequestMessage request, string? expectedNonce, string? authorityKey)
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
                await this.ProcessIdentityTokenAsync(tokenSet.IdentityToken, expectedNonce, authorityKey);
            }

            if (tokenSet?.AccessToken?.Length > 0)
            {
                await this.ProcessAccessTokenAsync(tokenSet.AccessToken, authorityKey);
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
                var authKey = await this.StorageFacade.GetAuthorityKeyAsync();
                var options = this.AuthOptions.Create(authKey ?? "");
                var requestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync(options);
                var request = requestBuilder
                    .WithClientId(options.ClientId)
                    .WithRefreshToken(refreshToken.Token)
                    .WithRedirectUri(this.Navigator.CurrentUri)
                    .Build();

                var result = await this.ExecuteTokenEndpointRequestAsync(request, null, authKey);
                return result.IsSuccess;
            }

            return false;
        }

    }
}

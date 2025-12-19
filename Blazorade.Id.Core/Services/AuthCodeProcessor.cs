using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The default implementation of the <see cref="IAuthCodeProcessor"/> service interface.
    /// </summary>
    public class AuthCodeProcessor : IAuthCodeProcessor
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public AuthCodeProcessor(
            IPropertyStore propertyStore, 
            ITokenStore tokenStore, 
            IEndpointService endpointService, 
            IHttpService httpService,
            IAuthenticationStateNotifier authStateNotifier,
            IOptions<JsonSerializerOptions> jsonOptions,
            IOptions<AuthorityOptions> authOptions,
            IRedirectUriProvider redirUriProvider,
            IScopeSorter scopeSorter,
            ITokenRefresher tokenRefresher
        ) {
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.HttpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.JsonOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.RedirUriProvider = redirUriProvider ?? throw new ArgumentNullException(nameof(redirUriProvider));
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
            this.TokenRefresher = tokenRefresher ?? throw new ArgumentNullException(nameof(tokenRefresher));
        }

        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly IEndpointService EndpointService;
        private readonly IHttpService HttpService;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly AuthorityOptions AuthOptions;
        private readonly IRedirectUriProvider RedirUriProvider;
        private readonly IScopeSorter ScopeSorter;
        private readonly ITokenRefresher TokenRefresher;

        /// <inheritdoc/>
        public async Task<bool> ProcessAuthorizationCodeAsync(string code)
        {
            bool result = false;
            string nonce = await this.PropertyStore.GetNonceAsync() ?? throw new NullReferenceException("Could not resolve nonce for the current authorization flow. Cannot use authorization code to acquire tokens without it.");
            string scope = await this.PropertyStore.GetScopeAsync() ?? throw new NullReferenceException("Could not resolve scope for the current authorization flow. Cannot use authorization code to acquire tokens without it.");
            string codeVerifier = await this.PropertyStore.GetCodeVerifierAsync() ?? throw new NullReferenceException("Could not resolve code verifier for the current authorization flow. Cannot use authorization code to acquire tokens without it.");

            // Now that we have read the values we can clean up the storage.
            await this.PropertyStore.RemoveNonceAsync();
            await this.PropertyStore.RemoveScopeAsync();
            await this.PropertyStore.RemoveCodeVerifierAsync();

            var redirUri = this.AuthOptions.RedirectUri ?? this.RedirUriProvider.GetRedirectUri().ToString();

            // First, we exchange the auth code for the initial set of tokens. In this exchange, we are
            // only interested in the refresh token. We will then use that refresh token to acquire both
            // access tokens and identity tokens using the token refresher service.
            result = await this.ExchangeAuthCodeAsync(code, this.AuthOptions.ClientId, codeVerifier, redirUri);
            if(result)
            {
                var sortedScopes = this.ScopeSorter.SortScopes(scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? []);
                if (sortedScopes.Count > 0)
                {
                    foreach (var item in sortedScopes)
                    {
                        await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions
                        {
                            Scopes = item.Value
                        });
                    }
                }
            }

            return result;
        }

        private async Task<bool> ExchangeAuthCodeAsync(string code, string clientId, string codeVerifier, string redirUri)
        {
            var now = DateTime.UtcNow;
            TokenResponse? tokenResponse = null;

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
            var tokenRequest = tokenRequestBuilder
                .WithAuthorizationCode(code)
                .WithClientId(this.AuthOptions.ClientId)
                .WithCodeVerifier(codeVerifier)
                .WithRedirectUri(redirUri)
                .Build();

            try
            {
                using (var response = await this.HttpService.SendRequestAsync(tokenRequest))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if(response.IsSuccessStatusCode)
                    {
                        tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, options: this.JsonOptions);
                        if (null != tokenResponse)
                        {
                            tokenResponse.ExpiresAtUtc = now.AddSeconds(tokenResponse.ExpiresIn);

                            if(tokenResponse.RefreshToken?.Length > 0)
                            {
                                await this.TokenStore.SetRefreshTokenAsync(tokenResponse.RefreshToken);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Authorization code exchange failed with status code {response.StatusCode}: {content}");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error while exchanging authorization code: {ex.Message}");
            }

            return false;
        }

        private async Task<bool> ProcessAuthCodeAsync(string code, string codeVerifier, string clientId, string resourceIdentifier, string scope, string redirUri)
        {
            var now = DateTime.UtcNow;
            TokenResponse? tokenResponse = null;

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
            var tokenRequest = tokenRequestBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(redirUri)
                .Build();

            try
            {
                using (var response = await this.HttpService.SendRequestAsync(tokenRequest))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, options: this.JsonOptions);
                        if (null != tokenResponse)
                        {
                            tokenResponse.ExpiresAtUtc = now.AddSeconds(tokenResponse.ExpiresIn);
                        }
                    }
                    else
                    {
                        var msg = $"Token request failed with status code {response.StatusCode}: {content}";
                    }
                }
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

            if(null != tokenResponse)
            {
                string? loginHint = null;
                JwtSecurityToken? idToken = null;
                // Before storing any tokens, we need to resolve the loginHint from the identity token, if one
                // is available in the token set. The loginHint is stored in the identity token in the preferred_username
                // claim.
                if (tokenResponse.IdentityToken?.Length > 0)
                {
                    try
                    {
                        idToken = new JwtSecurityToken(tokenResponse.IdentityToken);
                        loginHint = idToken.GetPreferredUsername();
                    }
                    catch { }
                }

                if (loginHint?.Length > 0)
                {
                    await this.PropertyStore.SetUsernameAsync(loginHint);
                }
                else
                {
                    await this.PropertyStore.RemoveUsernameAsync();
                }

                if (tokenResponse.RefreshToken?.Length > 0)
                {
                    await this.TokenStore.SetRefreshTokenAsync(tokenResponse.RefreshToken);
                }
                if (tokenResponse.AccessToken?.Length > 0)
                {
                    await this.TokenStore.SetAccessTokenAsync(resourceIdentifier, tokenResponse.AccessToken);
                }
                if (null != idToken)
                {
                    await this.TokenStore.SetIdentityTokenAsync(idToken);
                }

                // We update the authentication state 
                await this.AuthStateNotifier.StateHasChangedAsync();

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}

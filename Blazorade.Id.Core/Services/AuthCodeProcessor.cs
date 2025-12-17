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
            EndpointService endpointService, 
            IHttpClientFactory httpClientFactory,
            IAuthenticationStateNotifier authStateNotifier,
            IOptions<JsonSerializerOptions> jsonOptions,
            IOptions<AuthorityOptions> authOptions,
            IRedirectUriProvider redirUriProvider,
            IScopeSorter ScopeSorter
        ) {
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.JsonOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.RedirUriProvider = redirUriProvider ?? throw new ArgumentNullException(nameof(redirUriProvider));
            this.ScopeSorter = ScopeSorter ?? throw new ArgumentNullException(nameof(ScopeSorter));
        }

        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly EndpointService EndpointService;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly AuthorityOptions AuthOptions;
        private readonly IRedirectUriProvider RedirUriProvider;
        private readonly IScopeSorter ScopeSorter;

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

            var sortedScopes = this.ScopeSorter.SortScopes(scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? []);
            if(sortedScopes.Count > 0)
            {
                var key = sortedScopes.Keys.First();
                var scopes = sortedScopes[key];
                // The auth code that we process here is good only for one request to the token endpoint.
                // We can't reuse it to get all access tokens for all scope groups returned by the scope sorter.
                // What we do then is that we take the first scope group and use the auth code to get tokens for
                // that group. Then with the remaining groups we use refresh tokens to get access tokens for those.
                result = await this.ProcessAuthCodeAsync(code, codeVerifier, this.AuthOptions.ClientId, key, string.Join(' ', scopes), redirUri);
                if(result)
                {
                    // Now we process the rest of the scope groups with refresh tokens
                    // and remember to skip the first one. It would work also to refresh
                    // the first group, but that is unnecessary since we just exchanged
                    // the auth code for tokens for the scopes in the first group.
                    foreach(var item in sortedScopes.Skip(1))
                    {
                        
                    }
                }
                return result;
            }

            return result;
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

            var client = this.HttpClientFactory.CreateClient();
            try
            {
                using (var response = await client.SendAsync(tokenRequest))
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

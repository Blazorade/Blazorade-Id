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
            IRedirectUriProvider redirUriProvider
        ) {
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.JsonOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.RedirUriProvider = redirUriProvider ?? throw new ArgumentNullException(nameof(redirUriProvider));
        }

        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly EndpointService EndpointService;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly AuthorityOptions AuthOptions;
        private readonly IRedirectUriProvider RedirUriProvider;

        /// <inheritdoc/>
        public async Task<bool> ProcessAuthorizationCodeAsync(string code, GetTokenOptions? options = null)
        {
            options = options ?? new GetTokenOptions(); 
            options.Scopes = options.Scopes ?? this.AuthOptions.Scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

            var nonce = await this.PropertyStore.GetNonceAsync();
            var scope = await this.PropertyStore.GetScopeAsync();
            var codeVerifier = await this.PropertyStore.GetCodeVerifierAsync();

            var redirUri = this.AuthOptions.RedirectUri ?? this.RedirUriProvider.GetRedirectUri().ToString();

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync();
            var tokenRequest = tokenRequestBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(redirUri)
                .Build();

            await this.PropertyStore.RemoveNonceAsync();
            await this.PropertyStore.RemoveScopeAsync();
            await this.PropertyStore.RemoveCodeVerifierAsync();

            return await this.ProcessTokenRequestAsync(tokenRequest, nonce, scope);
        }

        private async Task<bool> ProcessTokenRequestAsync(HttpRequestMessage request, string? nonce, string? scope)
        {
            var now = DateTime.UtcNow;
            TokenSet? tokenSet = null;
            var client = this.HttpClientFactory.CreateClient();
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        tokenSet = JsonSerializer.Deserialize<TokenSet>(content, options: this.JsonOptions);
                        if(null != tokenSet)
                        {
                            tokenSet.ExpiresAtUtc = now.AddSeconds(tokenSet.ExpiresIn);
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
            }

            if(null != tokenSet)
            {
                string? loginHint = null;
                JwtSecurityToken? idToken = null;
                // Before storing any tokens, we need to resolve the loginHint from the identity token, if one
                // is available in the token set. The loginHint is stored in the identity token in the preferred_username
                // claim.
                if(tokenSet.IdentityToken?.Length > 0)
                {
                    try
                    {
                        idToken = new JwtSecurityToken(tokenSet.IdentityToken);
                        loginHint = idToken.GetPreferredUsername();
                    }
                    catch { }
                }

                if(loginHint?.Length > 0)
                {
                    await this.PropertyStore.SetUsernameAsync(loginHint);
                }

                var getOptions = new GetTokenOptions
                {
                    LoginHint = loginHint,
                    Scopes = scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []
                };

                if (tokenSet.RefreshToken?.Length > 0)
                {
                    await this.TokenStore.SetRefreshTokenAsync(tokenSet.RefreshToken, options: getOptions);
                }
                if (tokenSet.AccessToken?.Length > 0)
                {
                    await this.TokenStore.SetAccessTokenAsync(tokenSet.AccessToken, options: getOptions);
                }
                if(null != idToken)
                {
                    await this.TokenStore.SetIdentityTokenAsync(idToken, getOptions);
                }

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

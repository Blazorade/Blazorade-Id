using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service implementation that takes care of acquiring tokens on behalf of the application
    /// it is used in.
    /// </summary>
    public class BlazoradeIdService
    {
        public BlazoradeIdService(IOptionsFactory<AuthorityOptions> optionsFactory, IHttpClientFactory clientFactory, EndpointService epService, INavigator navigator, SerializationService serialization, StorageFactory storageFactory, CodeChallengeService codeChallengeService)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.EPService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.StorageFactory = storageFactory ?? throw new ArgumentNullException(nameof(storageFactory));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.SerializationService = serialization ?? throw new ArgumentNullException(nameof(serialization));
            this.CodeChallenge = codeChallengeService ?? throw new ArgumentNullException(nameof(codeChallengeService));
        }

        private readonly IOptionsFactory<AuthorityOptions> OptionsFactory;
        private readonly IHttpClientFactory ClientFactory;
        private readonly EndpointService EPService;
        private readonly StorageFactory StorageFactory;
        private readonly INavigator Navigator;
        private readonly SerializationService SerializationService;
        private readonly CodeChallengeService CodeChallenge;

        public async ValueTask<LoginCompletedState> CompleteLoginAsync(string authorizationCode, LoginState state)
        {
            var codeVerifier = await this.StorageFactory.SessionStorage.GetCurrentCodeVerifierAsync();
            var scope = await this.StorageFactory.SessionStorage.GetCurrentScopeAsync();
            var nonce = await this.StorageFactory.SessionStorage.GetCurrentNonceAsync();

            await this.StorageFactory.SessionStorage.RemoveCurrentCodeVerifierAsync();
            await this.StorageFactory.SessionStorage.RemoveCurrentScopeAsync();
            await this.StorageFactory.SessionStorage.RemoveCurrentNonceAsync();

            await this.StorageFactory.PersistentStorage.SetCurrentAuthorityKeyAsync(state.AuthorityKey);
            var authOptions = this.GetAuthOptions(state.AuthorityKey);
            var redirUri = this.CreateRedirectUri(authOptions);

            var tokenEndpointUri = await this.EPService.GetTokenEndpointAsync(authOptions) ?? throw new NullReferenceException("Unable to resolve token endpoint URI.");
            var tokenRequest = new TokenRequestBuilder(tokenEndpointUri)
                .WithClientId(authOptions.ClientId)
                .WithCodeVerifier(codeVerifier)
                .WithAuthorizationCode(authorizationCode)
                .WithScope(scope)
                .WithRedirectUri(redirUri)
                .Build();

            TokenSet tokens = null!;
            var completedState = new LoginCompletedState { ApplicationState = state.ApplicationState, AuthorityKey = state.AuthorityKey };
            var client = this.ClientFactory.CreateClient();
            using (var response = await client.SendAsync(tokenRequest))
            {
                var now = DateTime.UtcNow;
                var json = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                    tokens = JsonSerializer.Deserialize<TokenSet>(json) ?? throw new Exception($"Unable to deserialize response from token endpoint at '{tokenEndpointUri}'.");
                    tokens.ExpiresAtUtc = now.AddSeconds(tokens.ExpiresIn);
                    completedState.IsSuccess = true;
                }
                else
                {
                    completedState.Error = JsonSerializer.Deserialize<TokenError>(json) ?? throw new Exception($"Unable to deserialize the error response from the token endpoint at '{tokenEndpointUri}'");
                }
            }

            if(completedState.IsSuccess)
            {
                var idToken = new JwtSecurityToken(tokens.IdentityToken);
                completedState.DisplayName = idToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                completedState.Username = idToken.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value;

                if(completedState.Username?.Length > 0)
                {
                    var storage = this.StorageFactory.GetConfiguredStorage(authOptions);
                    await storage.SetCurrentUsernameAsync(completedState.Username);
                    await storage.SetCurrentTokenSetAsync(tokens);
                }
                else
                {
                    completedState.IsSuccess = false;
                    completedState.Error = new TokenError { ErrorDescription = $"Could not resolve username from identity token received from token endpoint at '{tokenEndpointUri}'." };
                }
            }

            return completedState;
        }

        /// <summary>
        /// Returns the username of the currently logged in user.
        /// </summary>
        public async ValueTask<string?> GetCurrentUsernameAsync()
        {
            var authorityKey = await this.StorageFactory.PersistentStorage.GetCurrentAuthorityKeyAsync();
            var loginHint = await this.StorageFactory
                .GetConfiguredStorage(authorityKey)
                .GetCurrentUsernameAsync();

            return loginHint?.Length > 0 ? loginHint : null;
        }

        public async ValueTask<ClaimsPrincipal?> GetCurrentUserPrincipalAsync()
        {
            ClaimsPrincipal? principal = null;
            var store = await this.GetCurrentConfiguredStorageAsync();
            var tokens = await store.GetCurrentTokenSetAsync(includeExpired: false);
            if(tokens?.IdentityToken?.Length > 0)
            {
                var idToken = new JwtSecurityToken(tokens.IdentityToken);
                var identity = new ClaimsIdentity(idToken.Claims, "Password", "name", "roles");
                principal = new ClaimsPrincipal(identity);
            }

            return principal;
        }

        /// <summary>
        /// Performs a login for the current user.
        /// </summary>
        /// <param name="scope">The scopes to request in the access token.</param>
        /// <param name="loginHint">A login hint. The specified value will be prefilled as username.</param>
        /// <param name="domainHint">
        /// A domain hint, i.e. <c>domain.com</c>. This allows the login process to skip e-mail verification
        /// and take the user directly to their home tenant for authentication. This is especially useful in
        /// federated situations or multi-tenant applications where your users may come for different tenants.
        /// </param>
        /// <param name="prompt">How to prompt the user during login.</param>
        /// <param name="authorityKey">
        /// The authority key to use to resolve the authorization endpoint where to send the user for authentication.
        /// This is the same key as you configure your authorities during application startup using the method
        /// <see cref="BlazoradeIdBuilder.AddAuthority"/>.
        /// </param>
        public async ValueTask LoginAsync(string? scope = "openid profile email", string? loginHint = null, string? domainHint = null, Prompt? prompt = null, string? responseType = null, string? nonce = null, object? state = null, string? authorityKey = null)
        {
            var authOptions = this.GetAuthOptions(authorityKey);
            var homeUri = new Uri(this.Navigator.HomeUri);
            var redirUri = this.CreateRedirectUri(authOptions);
            var currentUri = homeUri.MakeRelativeUri(new Uri(this.Navigator.CurrentUri));

            nonce = nonce ?? Guid.NewGuid().ToString();
            var codeVerifier = this.CodeChallenge.CreateCodeVerifier();

            await this.StorageFactory.SessionStorage.SetCurrentNonceAsync(nonce);
            await this.StorageFactory.SessionStorage.SetCurrentCodeVerifierAsync(codeVerifier);
            await this.StorageFactory.SessionStorage.SetCurrentScopeAsync(scope);

            var builder = await this.EPService.CreateAuthorizationUriBuilderAsync(authOptions);
            var authUri = builder
                .WithScope(scope)
                .WithLoginHint(loginHint)
                .WithDomainHint(domainHint)
                .WithPrompt(prompt)
                .WithResponseType(responseType ?? "code")
                .WithResponseMode(ResponseMode.Fragment)
                .WithRedirectUri(redirUri)
                .WithCodeChallenge(codeVerifier)
                .WithNonce(nonce)
                .WithState(this.SerializationService.SerializeToBase64String(new LoginState { ApplicationState = state, Uri = currentUri.ToString(), AuthorityKey = authorityKey }))
                .Build();

            await this.Navigator.NavigateToAsync(authUri);
        }

        public async ValueTask LogoutAsync(string? postLogoutRedirectUri = null, bool redirectToCurrentUri = true)
        {
            var authOptions = await this.GetCurrentAuthOptionsAsync();
            var username = await this.StorageFactory.GetConfiguredStorage(authOptions).GetCurrentUsernameAsync();

            var store = this.StorageFactory.GetConfiguredStorage(authOptions);
            await store.RemoveCurrentTokenSetAsync();
            await store.RemoveCurrentUsernameAsync();

            var builder = await this.EPService.CreateEndSessionUriBuilderAsync(authOptions);
            var logoutUri = builder
                .WithPostLogoutRedirectUri(
                    postLogoutRedirectUri?.Length > 0
                        ? postLogoutRedirectUri
                        : redirectToCurrentUri
                            ? this.Navigator.CurrentUri
                            : null
                )
                .WithLoginHint(username)
                .Build();

            await this.Navigator.NavigateToAsync(logoutUri);
        }



        private Uri CreateRedirectUri(AuthorityOptions authOptions)
        {
            Uri redirUri;
            var homeUri = new Uri(this.Navigator.HomeUri);

            if (authOptions.RedirectUri?.Length > 0)
            {
                var uri = new Uri(authOptions.RedirectUri, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                {
                    redirUri = uri;
                }
                else
                {
                    redirUri = new Uri(homeUri, uri.ToString());
                }
            }
            else
            {
                redirUri = homeUri;
            }

            return redirUri;
        }

        private AuthorityOptions GetAuthOptions(string? key)
        {
            var authOptions = this.OptionsFactory.Create(key ?? string.Empty);
            if (null == authOptions)
            {
                throw new ArgumentException("No authentication options found with the key specified in key.", nameof(key));
            }
            return authOptions;
        }

        private async ValueTask<AuthorityOptions> GetCurrentAuthOptionsAsync()
        {
            var authKey = await this.StorageFactory.PersistentStorage.GetCurrentAuthorityKeyAsync();
            return this.GetAuthOptions(authKey);
        }

        private async ValueTask<IStorage> GetCurrentConfiguredStorageAsync()
        {
            var authKey = await this.StorageFactory.PersistentStorage.GetCurrentAuthorityKeyAsync();
            var options = this.GetAuthOptions(authKey);
            return this.StorageFactory.GetConfiguredStorage(options);
        }
    }

}

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
        public BlazoradeIdService(IOptionsFactory<AuthorityOptions> optionsFactory, IHttpClientFactory clientFactory, EndpointService epService, ISessionStorage sessionStorage, IPersistentStorage persistentStorage, INavigator navigator)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.EPService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        private readonly IOptionsFactory<AuthorityOptions> OptionsFactory;
        private readonly IHttpClientFactory ClientFactory;
        private readonly EndpointService EPService;
        private readonly ISessionStorage SessionStorage;
        private readonly IPersistentStorage PersistentStorage;
        private readonly INavigator Navigator;

        public async ValueTask<LoginCompletedState> CompleteLoginAsync(string authorizationCode, LoginState state)
        {
            var codeVerifierKey = this.CreateCodeVerifierStorageKey();
            var codeVerifier = await this.SessionStorage.GetItemAsync<string>(codeVerifierKey);

            var scopeKey = this.CreateScopeStorageKey();
            var scope = await this.SessionStorage.GetItemAsync<string>(scopeKey);

            await this.SessionStorage.RemoveItemAsync(codeVerifierKey);

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

            RefreshableTokenSet tokens = null!;
            var completedState = new LoginCompletedState { ApplicationState = state.ApplicationState, AuthorityKey = state.AuthorityKey };
            var client = this.ClientFactory.CreateClient();
            using (var response = await client.SendAsync(tokenRequest))
            {
                var now = DateTime.UtcNow;
                var json = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                    tokens = JsonSerializer.Deserialize<RefreshableTokenSet>(json) ?? throw new Exception($"Unable to deserialize response from token endpoint at '{tokenEndpointUri}'.");
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
                    var storage = this.GetConfiguredStorage(authOptions);
                    var tokenStorageKey = this.CreateTokenSetKey(state.AuthorityKey, completedState.Username);
                    var usernameStorageKey = this.CreateCurrentUsernameStorageKey();

                    await storage.SetItemAsync(tokenStorageKey, tokens);
                    await storage.SetItemAsync(usernameStorageKey, completedState.Username);
                }
                else
                {
                    throw new Exception($"Could not resolve username from identity token received from token endpoint at '{tokenEndpointUri}'.");
                }
            }

            return completedState;
        }

        public TState DeserializeState<TState>(string base64) where TState : new()
        {
            var json = Base64UrlEncoder.Decode(base64);
            var state = JsonSerializer.Deserialize<TState>(json);
            return state ?? new TState();
        }

        public async ValueTask<TokenSet?> GetTokenSetSilentAsync(string scope = "openid profile offline_access")
        {
            TokenSet? tokens = await this.GetCurrentTokenSetAsync(includeExpired: false);
            return tokens;
        }

        /// <summary>
        /// Returns the username of the currently logged in user.
        /// </summary>
        public async ValueTask<string?> GetCurrentUsernameAsync()
        {
            var authorityKey = await this.GetCurrentAuthorityKeyAsync();
            var key = this.CreateCurrentUsernameStorageKey();
            var storage = this.GetConfiguredStorage(authorityKey);
            var loginHint = await storage.GetItemAsync<string>(key);

            return loginHint?.Length > 0 ? loginHint : null;
        }

        public async ValueTask<ClaimsPrincipal?> GetCurrentUserPrincipalAsync()
        {
            ClaimsPrincipal? principal = null;
            var tokens = await this.GetCurrentTokenSetAsync(includeExpired: false);
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
        public async ValueTask LoginAsync(string? scope = "openid profile offline_access", string? loginHint = null, string? domainHint = null, Prompt? prompt = null, object? state = null, string? authorityKey = null)
        {
            var authOptions = this.GetAuthOptions(authorityKey);
            var homeUri = new Uri(this.Navigator.HomeUri);
            var redirUri = this.CreateRedirectUri(authOptions);
            var currentUri = homeUri.MakeRelativeUri(new Uri(this.Navigator.CurrentUri));

            var codeVerifier = this.CreateCodeVerifier();
            var codeVerifierKey = this.CreateCodeVerifierStorageKey();
            await this.SessionStorage.SetItemAsync(codeVerifierKey, codeVerifier);

            var scopeKey = this.CreateScopeStorageKey();
            await this.SessionStorage.SetItemAsync(scopeKey, scope);

            var builder = await this.EPService.CreateAuthorizationUriBuilderAsync(authOptions);
            var authUri = builder
                .WithScope(scope)
                .WithLoginHint(loginHint)
                .WithDomainHint(domainHint)
                .WithPrompt(prompt)
                .WithResponseType(ResponseType.Code)
                .WithResponseMode(ResponseMode.Fragment)
                .WithRedirectUri(redirUri)
                .WithCodeChallenge(codeVerifier)
                .WithNonce(Guid.NewGuid().ToString())
                .WithState(this.SerializeState(new LoginState { ApplicationState = state, Uri = currentUri.ToString(), AuthorityKey = authorityKey }))
                .Build();

            await this.Navigator.NavigateToAsync(authUri);
        }

        /// <summary>
        /// Returns the timestamp in UTC when the tokens for the currently logged in user will expire.
        /// </summary>
        /// <returns>Returns the expiration timestamp in UTC or <c>null</c> if no user is currently logged in.</returns>
        /// <remarks>
        /// If a user has successfully logged in, this method will always return a timestamp, but that timestamp can
        /// be in the past.
        /// </remarks>
        public async ValueTask<DateTime?> TokensExpireUtcAsync()
        {
            var tokens = await this.GetCurrentTokenSetAsync();
            return tokens?.ExpiresAtUtc;
        }

        public async ValueTask LogoutAsync(string? postLogoutRedirectUri = null, bool redirectToCurrentUri = true)
        {
            var authKeyKey = this.CreateCurrentAuthorityKeyStorageKey();
            var authorityKey = await this.PersistentStorage.GetItemAsync<string>(authKeyKey);

            var authOptions = this.GetAuthOptions(authorityKey);
            var usernameKey = this.CreateCurrentUsernameStorageKey();
            var username = await this.GetCurrentUsernameAsync();
            var tokenSetKey = this.CreateTokenSetKey(authorityKey, username ?? string.Empty);

            var store = this.GetConfiguredStorage(authOptions);
            await store.RemoveItemAsync(usernameKey);
            await store.RemoveItemAsync(tokenSetKey);

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

        public string SerializeState(object state)
        {
            var json = JsonSerializer.Serialize(state);
            var base64 = Base64UrlEncoder.Encode(json);
            return base64;
        }




        private const string CodeVerifierChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        private Random Rnd = new Random();
        private string CreateCodeVerifier()
        {
            var length = this.Rnd.Next(43, 60);
            var arr = new char[length];

            for(int i = 0; i < length; i++)
            {
                var ix = this.Rnd.Next(0, CodeVerifierChars.Length - 1);
                arr[i] = CodeVerifierChars[ix];
            }

            return string.Join("", arr);
        }

        private string CreateCodeVerifierStorageKey()
        {
            return this.PrefixStorageKey(null, "codeVerifier");
        }

        private string CreateCurrentUsernameStorageKey()
        {
            return this.PrefixStorageKey(null, "currentUsername");
        }

        private string CreateCurrentAuthorityKeyStorageKey()
        {
            return this.PrefixStorageKey(null, "currentAuthKey");
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

        private string CreateScopeStorageKey()
        {
            return this.PrefixStorageKey(null, "scope");
        }

        private string CreateTokenSetKey(string? authKey, string username)
        {
            return PrefixStorageKey(authKey, $"{username}.tokenSet");
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

        private IStorage GetConfiguredStorage(AuthorityOptions authOptions)
        {
            return authOptions.CacheMode == TokenCacheMode.Session
                ? (IStorage)this.SessionStorage 
                : (IStorage)this.PersistentStorage;


        }

        private IStorage GetConfiguredStorage(string? key)
        {
            var options = this.GetAuthOptions(key);
            return this.GetConfiguredStorage(options);
        }

        /// <summary>
        /// Returns the authority key that the currently logged on user was logged in with.
        /// </summary>
        /// <returns></returns>
        private async ValueTask<string?> GetCurrentAuthorityKeyAsync()
        {
            var storageKey = this.CreateCurrentAuthorityKeyStorageKey();
            return await this.PersistentStorage.GetItemAsync<string>(storageKey);
        }

        private async ValueTask<TokenSet?> GetCurrentTokenSetAsync(bool includeExpired = false)
        {
            var authKey = await this.GetCurrentAuthorityKeyAsync();
            var username = await this.GetCurrentUsernameAsync();
            return await this.GetCurrentTokenSetAsync(authKey, username, includeExpired: includeExpired);
        }

        private async ValueTask<TokenSet?> GetCurrentTokenSetAsync(string? authKey, string? username, bool includeExpired = false)
        {
            TokenSet? tokens = null;
            if(username?.Length > 0)
            {
                var key = this.CreateTokenSetKey(authKey, username);
                var set = await this.GetConfiguredStorage(authKey).GetItemAsync<TokenSet?>(key);
                if(null != set && (set.ExpiresAtUtc > DateTime.UtcNow || includeExpired))
                {
                    tokens = set;
                }
            }

            return tokens;
        }

        private string PrefixStorageKey(string? authKey, string key)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

    }

}

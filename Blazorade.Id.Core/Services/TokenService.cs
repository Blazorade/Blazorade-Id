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
            IEndpointService epService, 
            IOptions<JsonSerializerOptions> jsonOptions,
            IAuthCodeProvider authCodeProvider,
            IAuthCodeProcessor authCodeProcessor,
            IRedirectUriProvider redirectProvider,
            IScopeSorter scopeSorter,
            ITokenRefresher tokenRefresher
        ) {
            this.HttpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.AuthOptions = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.EndpointService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.JsonOptions = jsonOptions.Value ?? throw new ArgumentNullException(nameof(jsonOptions));
            this.AuthCodeProvider = authCodeProvider ?? throw new ArgumentNullException(nameof(authCodeProvider));
            this.AuthCodeProcessor = authCodeProcessor ?? throw new ArgumentNullException(nameof(authCodeProcessor));
            this.RedirectUriProvider = redirectProvider ?? throw new ArgumentNullException(nameof(redirectProvider));
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
            this.TokenRefresher = tokenRefresher ?? throw new ArgumentNullException(nameof(tokenRefresher));
        }

        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly AuthorityOptions AuthOptions;
        private readonly IEndpointService EndpointService;
        private readonly JsonSerializerOptions JsonOptions;
        private readonly IAuthCodeProvider AuthCodeProvider;
        private readonly IAuthCodeProcessor AuthCodeProcessor;
        private readonly IRedirectUriProvider RedirectUriProvider;
        private readonly IScopeSorter ScopeSorter;
        private readonly ITokenRefresher TokenRefresher;


        /// <summary>
        /// Returns the access token for the current signed in user.
        /// </summary>
        /// <param name="options">The options for getting the access token.</param>
        /// <remarks>
        /// This service will attempt to refresh the access token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<AccessTokenDictionary> GetAccessTokenAsync(GetTokenOptions? options = null)
        {
            var result = new AccessTokenDictionary();
            options = await this.GetTokenOptionsAsync(options);
            ScopeDictionary sortedScopes = this.ScopeSorter.SortScopes(options.Scopes ?? []);
            foreach(var item in sortedScopes)
            {
                var container = await this.TokenStore.GetAccessTokenAsync(item.Key);

                if (
                    !options.Prompt.RequiresInteraction()
                    && (
                        null == container
                        || container.Expires < DateTime.UtcNow
                        || container?.AcquisitionOptions?.ContainsScopes(item.Value.ToArray()) == false
                    )
                )
                {
                    // Set the container to null, since at this point, it is not valid, and must be renewed somehow.
                    container = null;

                    // No container was found, or it has expired, or it does not contain all requested scopes.
                    if (await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions { Scopes = item.Value.Select(x => x.ToString()) }))
                    {
                        container = await this.TokenStore.GetAccessTokenAsync(item.Key);
                    }
                }

                if(null == container)
                {
                    // If the container is still null, we need to acquire it interactively. At this point, we will use
                    // all of the scopes requested by the caller, since we want to have the user consent to all of them.
                    var code = await this.AuthCodeProvider.GetAuthorizationCodeAsync(options);
                    if(code?.Length > 0)
                    {
                        var processed = await this.AuthCodeProcessor.ProcessAuthorizationCodeAsync(code);
                        if (processed)
                        {
                            container = await this.TokenStore.GetAccessTokenAsync(item.Key);
                        }
                    }
                }

                if (null != container)
                {
                    result[item.Key] = container;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the identity token for the current signed in user.
        /// </summary>
        /// <param name="options">The options for getting the identity token.</param>
        /// <remarks>
        /// This service will attempt to refresh the identity token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<JwtSecurityToken?> GetIdentityTokenAsync(GetTokenOptions? options = null)
        {
            options = await this.GetTokenOptionsAsync(options);
            var container = await this.TokenStore.GetIdentityTokenAsync();
            var token = await this.GetValidTokenAsync(this.TokenStore.GetIdentityTokenAsync, options);
            return token;
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

        /// <summary>
        /// Uses the given <paramref name="tokenGetter"/> to get a valid token. If a valid token is not found,
        /// then the service will attempt to refresh the tokens using a refresh token, if available. If refresh
        /// is successful, the valid token is returned using the same <paramref name="tokenGetter"/>.
        /// </summary>
        /// <param name="tokenGetter">A delegate that is used to get a token from storage.</param>
        /// <param name="options">Options that control how the token is acquired.</param>
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
                    bool canRefresh = true;

                    if(options.Scopes?.Count() > 0)
                    {
                        // First we need to determine if refreshing is possible at all. In order for the refresh to
                        // be successful, we need to have acquired a token with the same scopes as we are requesting now.
                        var requestedScopes = options.Scopes != null ? string.Join(' ', options.Scopes) : null;
                    }

                    // Try to refresh tokens silently first.
                    if (canRefresh && await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions { Scopes = options.Scopes ?? [] }))
                    {
                        tokenContainer = await tokenGetter();
                        token = tokenContainer?.GetToken(options.Scopes);
                    }
                }

                if (null == token)
                {
                    // If the token still is null, then we need to aquire it interactively.
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

        private async Task<GetTokenOptions> GetTokenOptionsAsync(GetTokenOptions? options)
        {
            options = options ?? new GetTokenOptions();
            options.Scopes = options.Scopes ?? $"{this.AuthOptions.Scope}".Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if(string.IsNullOrEmpty(options.LoginHint))
            {
                options.LoginHint = await this.PropertyStore.GetUsernameAsync();
            }

            return options;
        }
    }
}

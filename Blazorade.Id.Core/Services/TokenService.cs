using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blazorade.Id.Services
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
            IOptions<AuthorityOptions> authOptions, 
            ITokenStore tokenStore,
            IPropertyStore propertyStore,
            IAuthorizationCodeProvider authCodeProvider,
            IAuthorizationCodeProcessor authCodeProcessor,
            IScopeSorter scopeSorter,
            ITokenRefresher tokenRefresher,
            IAuthorizationCodeFailureNotifier authCodeFailureHandler
        ) {
            this.AuthOptions = authOptions.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.AuthCodeProvider = authCodeProvider ?? throw new ArgumentNullException(nameof(authCodeProvider));
            this.AuthCodeProcessor = authCodeProcessor ?? throw new ArgumentNullException(nameof(authCodeProcessor));
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
            this.TokenRefresher = tokenRefresher ?? throw new ArgumentNullException(nameof(tokenRefresher));
            this.AuthCodeFailureHandler = authCodeFailureHandler ?? throw new ArgumentNullException(nameof(authCodeFailureHandler));
        }

        private readonly IPropertyStore PropertyStore;
        private readonly ITokenStore TokenStore;
        private readonly AuthorityOptions AuthOptions;
        private readonly IAuthorizationCodeProvider AuthCodeProvider;
        private readonly IAuthorizationCodeProcessor AuthCodeProcessor;
        private readonly IScopeSorter ScopeSorter;
        private readonly ITokenRefresher TokenRefresher;
        private readonly IAuthorizationCodeFailureNotifier AuthCodeFailureHandler;


        /// <summary>
        /// Returns the access token for the current signed in user.
        /// </summary>
        /// <param name="options">The options for getting the access token.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <remarks>
        /// This service will attempt to refresh the access token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<AccessTokenDictionary> GetAccessTokensAsync(GetTokenOptions? options = null, CancellationToken cancellationToken = default)
        {
            var result = new AccessTokenDictionary();
            options = await this.GetTokenOptionsAsync(options);
            ScopeDictionary sortedScopes = await this.ScopeSorter.SortScopesAsync(options.Scopes ?? []);
            bool allowInteraction = options.Prompt != Prompt.None; // Any other prompt than None may potentially use interaction.

            foreach(var item in from x in sortedScopes where x.Value.ContainsResourceScopes() select x)
            {
                // Since we are getting access tokens, we only enumerate those scopes that are
                // associated with resources. A group with only Open ID scopes will be skipped.

                TokenContainer? container = null;

                if(!options.Prompt.RequiresInteraction())
                {
                    container = await this.TokenStore.GetAccessTokenAsync(item.Key);
                }

                if (!this.IsTokenContainerValid(container, item.Value.Values()) && !options.Prompt.RequiresInteraction())
                {
                    // The container is in some way not valid for the current request. There was either no container found,
                    // it was expired, or it did not contain all the scopes that were requested.
                    // So, we attempt to refresh the tokens silently first.
                    // We can refresh tokens silently only if the prompt option does not require interaction.

                    // Set the container to null, since at this point, it is not valid, and must be renewed somehow.
                    container = null;

                    // No container was found, or it has expired, or it does not contain all requested scopes, so we
                    // need to refresh it.
                    if (await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions { Scopes = item.Value.Select(x => x.ToString()) }))
                    {
                        container = await this.TokenStore.GetAccessTokenAsync(item.Key);
                    }
                }

                if(null == container)
                {
                    // If the container is still null, we need to acquire it interactively. At this point, we will use
                    // all of the scopes requested by the caller, since we want to have the user consent to all of them.
                    // However, we will only do this if interaction is allowed by the prompt option. This is because we
                    // might be enumerating multiple scopes, and we should use interaction only once. Otherwise we might
                    // end up in troubles with multiple prompts to the user, or browsers blocking pop-ups etc.
                    if (allowInteraction && await this.AcquireTokensInteractiveAsync(options))
                    {
                        container = await this.TokenStore.GetAccessTokenAsync(item.Key);

                        // We successfully acquired tokens interactively, we don't have to do that
                        // again for the next sorted group of scopes because when prompting the user
                        // they will be asked to consent to all scopes requested, not just the group
                        // of scopes we are processing for item.
                        options.Prompt = null;
                    }

                    allowInteraction = false;
                }

                var token = container?.ParseToken();
                if(null != token)
                {
                    result[item.Key] = token;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the identity token for the current signed in user.
        /// </summary>
        /// <param name="options">The options for getting the identity token.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <remarks>
        /// This service will attempt to refresh the identity token in case the previously stored token
        /// has expired, but a refresh token is available.
        /// </remarks>
        public async Task<JwtSecurityToken?> GetIdentityTokenAsync(GetTokenOptions? options = null, CancellationToken cancellationToken = default)
        {
            options = await this.GetTokenOptionsAsync(options);
            TokenContainer? container = null;


            if (!options.Prompt.RequiresInteraction())
            {
                container = await this.TokenStore.GetIdentityTokenAsync();
            }

            // Because we're interested in just the identity token, we pick the scopes that are
            // associated with Open ID.
            var openIdScopes = from x in options.Scopes ?? [] where ScopeList.OpenIdScopes.Select(x => x.Value).Contains(x) select x;

            if (!this.IsTokenContainerValid(container, openIdScopes) && !options.Prompt.RequiresInteraction())
            {
                container = null;
                if(await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions { Scopes = openIdScopes.ToArray() }))
                {
                    container = await this.TokenStore.GetIdentityTokenAsync();
                }
            }

            if(null == container && await this.AcquireTokensInteractiveAsync(options))
            {
                container = await this.TokenStore.GetIdentityTokenAsync();
            }

            return container?.ParseToken();
        }



        private async Task<bool> AcquireTokensInteractiveAsync(GetTokenOptions options)
        {
            // If the prompt option is None, we should not attempt to acquire tokens interactively.
            if (options.Prompt == Prompt.None) return false;

            var codeResult = await this.AuthCodeProvider.GetAuthorizationCodeAsync(options);
            if(codeResult?.Code?.Length > 0)
            {
                return await this.AuthCodeProcessor.ProcessAuthorizationCodeAsync(codeResult.Code);
            }
            else if(null != codeResult?.FailureReason)
            {
                await this.AuthCodeFailureHandler.HandleFailureAsync(codeResult);
            }

            return false;
        }

        private bool IsTokenContainerValid(TokenContainer? container, IEnumerable<string> requiredScopes)
        {
            if (null == container) return false;
            if (container.Expires < DateTime.UtcNow) return false;
            if (!container.ContainsScopes(requiredScopes.ToArray())) return false;

            return true;
        }

        private async Task<GetTokenOptions> GetTokenOptionsAsync(GetTokenOptions? options)
        {
            options = options ?? new GetTokenOptions();
            options.Scopes = options.Scopes ?? $"{this.AuthOptions.Scope}".Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if(string.IsNullOrEmpty(options.LoginHint))
            {
                options.LoginHint = await this.PropertyStore.GetLoginHintAsync();
            }

            return options;
        }
    }
}

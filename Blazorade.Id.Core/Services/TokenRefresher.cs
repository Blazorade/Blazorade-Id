using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The default token refresher provided by Blazorade Id.
    /// </summary>
    public class TokenRefresher : ITokenRefresher
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public TokenRefresher(
            IScopeSorter scopeSorter,
            ITokenStore tokenStore,
            IRedirectUriProvider redirectUriProvider,
            IEndpointService endpointService,
            IHttpService httpService,
            IAuthenticationStateNotifier authStateNotifier,
            IOptions<AuthorityOptions> authOptions
        )
        {
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.RedirUriProvider = redirectUriProvider ?? throw new ArgumentNullException(nameof(redirectUriProvider));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.HttpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly IScopeSorter ScopeSorter;
        private readonly ITokenStore TokenStore;
        private readonly IRedirectUriProvider RedirUriProvider;
        private readonly IEndpointService EndpointService;
        private readonly IHttpService HttpService;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly AuthorityOptions AuthOptions;

        /// <inheritdoc/>
        public async Task<bool> RefreshTokensAsync(TokenRefreshOptions options, CancellationToken cancellationToken = default)
        {
            bool result = false;

            // If the refresh token has been set in the options, then we MUST use that one.
            // If not specified, we try to read it from the token store.
            var refreshToken = options.RefreshToken?.Length > 0 
                ? new TokenContainer(options.RefreshToken) : 
                null 
                ?? await this.TokenStore.GetRefreshTokenAsync();

            if(refreshToken?.Token?.Length > 0)
            {
                result = true;
                var redirUri = this.AuthOptions.RedirectUri ?? this.RedirUriProvider.GetRedirectUri().ToString();
                var sortedScopes = await this.ScopeSorter.SortScopesAsync(options.Scopes, cancellationToken);
                foreach (var item in sortedScopes)
                {
                    var builder = await this.EndpointService.CreateTokenRequestBuilderAsync();
                    var request = builder
                        .WithRedirectUri(redirUri)
                        .WithClientId(this.AuthOptions.ClientId)
                        .WithRefreshToken(refreshToken.Token)
                        .WithScope(string.Join(' ', item.Value))
                        .Build();

                    try
                    {
                        var now = DateTime.UtcNow;
                        using (var response = await this.HttpService.SendRequestAsync(request, cancellationToken))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(content);
                                if (null != tokenResponse)
                                {
                                    tokenResponse.ExpiresAtUtc = now.AddSeconds(tokenResponse.ExpiresIn);
                                    if(tokenResponse.RefreshToken?.Length > 0)
                                    {
                                        await this.TokenStore.SetRefreshTokenAsync(new TokenContainer
                                        {
                                            Token = tokenResponse.RefreshToken
                                        });
                                    }

                                    if(tokenResponse.IdentityToken?.Length > 0 && item.Value.ContainsOpenIdScopes())
                                    {
                                        // We have a potential identity token to store. Before storing it though,
                                        // we must ensure that the <c>aud</c> claim matches the client ID of
                                        // the currently configured application. Identity tokens are always issued
                                        // to the client application that requested them.
                                        try
                                        {
                                            var jwt = new JwtSecurityToken(tokenResponse.IdentityToken);
                                            if(jwt.Audiences.Contains(this.AuthOptions.ClientId))
                                            {
                                                var container = new TokenContainer
                                                {
                                                    Token = tokenResponse.IdentityToken,
                                                    Expires = tokenResponse.ExpiresAtUtc,
                                                    Scopes = item.Value
                                                };

                                                await this.TokenStore.SetIdentityTokenAsync(container);
                                                await this.AuthStateNotifier.StateHasChangedAsync();
                                            }
                                        }
                                        catch { }
                                    }

                                    if(tokenResponse.AccessToken?.Length > 0 && item.Value.ContainsResourceScopes())
                                    {
                                        // For access tokens, we must make sure that the scopes being requested in the
                                        // refresh contain also resource scopes, and not just OpenID scopes. If only
                                        // OpenID scopes are requested, we cannot be sure which resource the access
                                        // token is issued to.
                                        
                                        var container = new TokenContainer
                                        {
                                            Token = tokenResponse.AccessToken,
                                            Expires = tokenResponse.ExpiresAtUtc,
                                            Scopes = item.Value
                                        };

                                        await this.TokenStore.SetAccessTokenAsync(item.Key, container);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"The response from the token endpoint was not successful. Status code: {response.StatusCode}");
                                result = false;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error while communicating with the token endpoint: {ex.Message}");
                        result = false;
                    }
                }

            }

            return result;
        }
    }
}

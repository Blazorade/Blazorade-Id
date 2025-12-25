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
    /// Produces <see cref="HttpRequestMessage"/> instances that are configured with the appropriate bearer token
    /// matching the specified scopes.
    /// </summary>
    public class HttpRequestFactory : IHttpRequestFactory
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public HttpRequestFactory(ITokenService tokenService, IScopeSorter scopeSorter)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
        }

        private readonly ITokenService TokenService;
        private readonly IScopeSorter ScopeSorter;

        /// <inheritdoc/>
        public async Task<HttpRequestMessage?> CreateRequestAsync(string requestUri, HttpMethod method, IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            JwtSecurityToken? token = null;
            HttpRequestMessage? request = null;
            var sorted = await this.ScopeSorter.SortScopesAsync(scopes, cancellationToken);

            if(sorted.Count > 0)
            {
                var tokens = await this.TokenService.GetAccessTokensAsync(new Model.GetTokenOptions { Scopes = sorted.First().Value.Values() }, cancellationToken);
                token = tokens.Count > 0 ? tokens.First().Value : null;
            }

            if (null != token)
            {
                request = new HttpRequestMessage(method, requestUri);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.RawData);
            }

            return request;
        }
    }
}

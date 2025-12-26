using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A request factory for creating HTTP requests that are configured with the appropriate bearer token
    /// for a specific resource and given scopes.
    /// </summary>
    public interface IHttpRequestFactory
    {
        /// <summary>
        /// Creates an <see cref="HttpRequestMessage"/> for the specified resource URI and scopes.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="scopes">
        /// <para>The scopes that the bearer token must have.</para>
        /// <para>
        /// If the given tokens specify scopes that belong to different resources, the http request
        /// produced uses the access token to the resource that is associated with the first scope in the list.
        /// </para>
        /// <para>
        /// All scopes that are associated with the same resource as the first scope will be included in the
        /// access token specified as the bearer token in the produced HTTP request.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// The method returns either the <see cref="HttpRequestMessage"/> created from the specified parameters,
        /// or <see langword="null"/> if a suitable access token could not be resolved.
        /// </returns>
        Task<HttpRequestMessage?> CreateRequestAsync(string requestUri, HttpMethod method, IEnumerable<string> scopes, CancellationToken cancellationToken = default);
    }
}

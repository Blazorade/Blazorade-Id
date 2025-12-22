using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Defines the interface for a service that performs HTTP operations.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Sends the specified HTTP request asynchronously and returns the response.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The HTTP response message.</returns>
        Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request);
    }
}

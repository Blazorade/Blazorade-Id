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
    /// All HTTP requests in Blazorade Id are performed through this service.
    /// </summary>
    /// <remarks>
    /// If you want to customize how HTTP requests are performed, you can implement 
    /// your own version of this service by implementing the <see cref="IHttpService"/>
    /// interface.
    /// </remarks>
    public class HttpService : IHttpService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public HttpService(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private readonly IHttpClientFactory HttpClientFactory;

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var client = this.HttpClientFactory.CreateClient();
            return await client.SendAsync(request, cancellationToken);
        }
    }
}

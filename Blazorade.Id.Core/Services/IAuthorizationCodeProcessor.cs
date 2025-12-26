using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The service interface for services that process authorization codes provided
    /// by implementations of the <see cref="IAuthorizationCodeProvider"/> interface.
    /// </summary>
    public interface IAuthorizationCodeProcessor
    {
        /// <summary>
        /// Processes the specified authorization code and returns a value indicating 
        /// whether the processing was successful.
        /// </summary>
        /// <param name="code">The authorization code to process.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>Returns either <see langword="true"/> or <see langword="false"/>.</returns>
        Task<bool> ProcessAuthorizationCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}

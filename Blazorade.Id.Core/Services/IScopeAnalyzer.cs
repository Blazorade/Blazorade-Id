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
    /// The service interface for a scope analyzer.
    /// </summary>
    /// <remarks>
    /// A scope analyzer is responsible for analyzing scopes and sorting Oauth2/OpenID Connect scopes into groups 
    /// that represent the target resource they are intended for. Each scope is also classified so that it potentially
    /// can be treated differently based on the scope's classification. Each target resource that an application 
    /// connects to must have its own access token.
    /// </remarks>
    public interface IScopeAnalyzer
    {
        /// <summary>
        /// Responseible for analyzing the provided scopes into groups representing the target resources they belong to.
        /// </summary>
        /// <param name="scopes">The scopes to analyze.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// Returns a dictionary where the identifier for the target resource is the key, and the list of scopes
        /// belonging to that resource is the value.
        /// </returns>
        Task<ScopeDictionary> AnalyzeScopesAsync(IEnumerable<string> scopes, CancellationToken cancellationToken = default);
    }
}

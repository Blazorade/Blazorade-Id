using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Defines the interface that navigators must implement.
    /// </summary>
    /// <remarks>
    /// Navigators assist the application to navigate to various addresses.
    /// </remarks>
    public interface INavigator
    {
        /// <summary>
        /// Returns home URI of the application.
        /// </summary>
        string? HomeUri { get; }

        /// <summary>
        /// Returns the current URI.
        /// </summary>
        string? CurrentUri { get; }

        /// <summary>
        /// Navigates to the given URI.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        ValueTask NavigateToAsync(string uri);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The service contract for services that provide redirect URIs for authentication flows.
    /// </summary>
    public interface IRedirectUriProvider
    {
        /// <summary>
        /// Returns the redirect URI to be used in authentication flows.
        /// </summary>
        Uri GetRedirectUri();
    }
}

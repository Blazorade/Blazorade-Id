using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A refresh token store that stores refresh tokens in the browser's session storage.
    /// </summary>
    /// <remarks>
    /// Storing tokens in a browser's session storage makes them more vulnerable to XSS attacks.
    /// This is especially true for refresh tokens, which are long-lived tokens that can be used 
    /// to obtain new access tokens.
    /// </remarks>
    public class BrowserSessionStorageRefreshTokenStore : WebRefreshTokenStoreBase
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public BrowserSessionStorageRefreshTokenStore(IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(WebStoreType.SessionStorage, jsRuntime, jsonOptions)
        {
        }

    }
}

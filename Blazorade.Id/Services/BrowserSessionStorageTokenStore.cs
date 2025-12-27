using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A token store that stores tokens in the browser's session storage.
    /// </summary>
    public class BrowserSessionStorageTokenStore : WebTokenStoreBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BrowserSessionStorageTokenStore"/> class.
        /// </summary>
        public BrowserSessionStorageTokenStore(IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(WebStoreType.SessionStorage, jsRuntime, jsonOptions)
        {
        }

    }
}

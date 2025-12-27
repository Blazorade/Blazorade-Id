using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A token store that uses the browser local storage to persist tokens across sessions.
    /// </summary>
    public class BrowserLocalStorageTokenStore : WebTokenStoreBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BrowserLocalStorageTokenStore"/> class.
        /// </summary>
        public BrowserLocalStorageTokenStore(IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(WebStoreType.LocalStorage, jsRuntime, jsonOptions)
        {
        }

    }
}

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
    /// A property store implementation that stores properties in the browser's local storage.
    /// </summary>
    public class BrowserLocalStoragePropertyStore : WebPropertyStoreBase
    {
        /// <inheritdoc/>
        public BrowserLocalStoragePropertyStore(IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(Model.WebStoreType.LocalStorage, jsRuntime, jsonOptions)
        {
        }

    }
}

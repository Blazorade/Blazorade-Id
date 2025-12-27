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
    /// A property store implementation that stores properties in the browser's session storage.
    /// </summary>
    public class BrowserSessionStoragePropertyStore : WebPropertyStoreBase
    {
        /// <inheritdoc/>
        public BrowserSessionStoragePropertyStore(IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(WebStoreType.SessionStorage, jsRuntime, jsonOptions)
        {
        }
    }
}

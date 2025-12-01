using Blazorade.Core.Interop;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    public class BlazoradeIdScriptService
    {
        public BlazoradeIdScriptService(IJSRuntime jsRuntime)
        {
            this.JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        private readonly IJSRuntime JsRuntime;


        internal async Task<DotNetInstanceCallbackHandler<TResult>> CreateCallbackHandlerAsync<TResult>(string functionName, Dictionary<string, object>? data = null)
        {
            var module = await this.GetBlazoradeIdModuleAsync();
            return new DotNetInstanceCallbackHandler<TResult>(module, functionName, data);
        }

        internal async Task InvokeVoidAsync(string functionName, params object?[]? args)
        {
            var module = await this.GetBlazoradeIdModuleAsync();
            await module.InvokeVoidAsync(functionName, args: args);
        }

        private IJSObjectReference _BlazoradeIdModule = null!;
        /// <summary>
        /// Returns a reference to the Blazorade Id JavaScript file.
        /// </summary>
        private async Task<IJSObjectReference> GetBlazoradeIdModuleAsync()
        {
            return _BlazoradeIdModule ??= await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Blazorade.Id/js/blazoradeId.js");
        }

    }
}

using Blazorade.Id.Core.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    public class BlazorAuthCodeProvider : IAuthCodeProvider
    {
        public BlazorAuthCodeProvider(IJSRuntime jsRuntime)
        {
            this.JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        private readonly IJSRuntime JsRuntime;

        public async Task<string?> GetAuthorizationCodeAsync()
        {
            await this.JsRuntime.InvokeVoidAsync("open", "/.", "blazoradeauth", "scrollbars=no,resizable=no,status=no,location=no,toolbar=no,menubar=no,width=480,height=640,left=200,top=200");
            throw new NotImplementedException();
        }
    }
}

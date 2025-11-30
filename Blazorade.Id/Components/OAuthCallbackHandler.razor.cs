using Blazorade.Core.Interop;
using Blazorade.Id.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class OAuthCallbackHandler
    {

        [Inject]
        private NavigationManager NavMan { get; set; } = null!;

        [Inject]
        private BlazoradeIdScriptService ScriptService { get; set; } = null!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                var data = new Dictionary<string, object>
                {
                    { "url", this.NavMan.Uri }
                };

                try
                {
                    using (var handler = await this.ScriptService.CreateCallbackHandlerAsync<bool>("signalAuthorizationPopupResponseUrl", data))
                    {
                        var result = await handler.GetResultAsync();
                    }
                }
                catch(FailureCallbackException ex)
                {
                    var msg = ex.Message;
                    var result = ex.Result;
                }
                catch (InteropTimeoutException ex)
                {
                    var msg = ex.Message;
                }
            }
        }
    }
}

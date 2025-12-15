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
    /// <summary>
    /// A Blazor component that handles OAuth callback responses.
    /// </summary>
    partial class OAuthCallbackHandler
    {

        [Inject]
        private NavigationManager NavMan { get; set; } = null!;

        [Inject]
        private BlazoradeIdScriptService ScriptService { get; set; } = null!;


        /// <summary>
        /// Handles communication with the parent window.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                try
                {
                    Dictionary<string, object> input = new Dictionary<string, object>
                    {
                        { "responseUrl", this.NavMan.Uri }
                    };

                    using(var handler = await this.ScriptService.CreateCallbackHandlerAsync<bool>("signalAuthorizationPopupResponseUrl", data: input))
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
                catch(Exception ex)
                {
                    var msg = ex.Message;
                }
            }
        }
    }
}

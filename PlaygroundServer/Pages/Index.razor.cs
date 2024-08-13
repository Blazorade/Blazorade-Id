using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PlaygroundServer.Pages
{
    partial class Index
    {

        [Inject]
        TokenService TokenService { get; set; }

        private async Task LoginOnClickAsync(MouseEventArgs args)
        {
            var idToken = await this.TokenService.GetIdentityTokenAsync();
        }
    }
}

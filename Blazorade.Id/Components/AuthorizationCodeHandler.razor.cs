using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class AuthorizationCodeHandler
    {

        [Inject]
        public BlazoradeIdService BidService { get; set; } = null!;

        [Inject]
        public NavigationManager NavMan { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var uri = new Uri(this.NavMan.Uri);
            var query = uri.Fragment?.Length > 0 ? uri.Fragment.Substring(1) : uri.Query;
            if(query?.Length > 0)
            {
                var parameters = QueryHelpers.ParseNullableQuery(query) ?? new Dictionary<string, StringValues>();
                if (parameters.ContainsKey("code"))
                {
                    LoginState? state = null;
                    if(parameters.ContainsKey("state"))
                    {
                        var json = Base64UrlEncoder.Decode(parameters["state"].ToString());
                        state = JsonSerializer.Deserialize<LoginState>(json, options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    }

                    var code = parameters["code"].ToString();

                    await this.BidService.CompleteLoginAsync(code, state);
                }
            }
        }
    }
}

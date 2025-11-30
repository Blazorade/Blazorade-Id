using Blazorade.Id.Components.Pages;
using Blazorade.Id.Core.Services;
using Blazorade.Id.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazoradeCore()
    .AddScoped<BlazoradeIdScriptService>()
    .AddBlazoradeIdWasmApplication()
        .AddAuthority((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            config.GetRequiredSection("blazorade:id").Bind(options);

            if(string.IsNullOrEmpty(options.RedirectUri))
            {
                var navMan = sp.GetRequiredService<NavigationManager>();
                options.RedirectUri = new Uri(new Uri(navMan.BaseUri), OAuthCallback.RoutePath).ToString();
            }
        })
    ;

await builder.Build().RunAsync();

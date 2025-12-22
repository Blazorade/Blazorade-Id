using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazoradeIdWasmApplication()
        .AddAuthority((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            config.GetRequiredSection("blazorade:id").Bind(options);
        })


    // By default, Blazorade ID uses in-memory token storage, which means tokens are lost when the page is refreshed.
    // If you want to persist tokens across page refreshes, you can use one of the token stores below.
    //.AddTokenStore<BlazorSessionTokenStore>()
    //.AddTokenStore<BlazorPersistentTokenStore>()
    ;

await builder.Build().RunAsync();

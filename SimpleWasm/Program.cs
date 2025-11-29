using Blazorade.Id.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazoradeCore()
    .AddBlazoradeIdWasmApplication()
        .AddPropertyStorage<BlazorSessionPropertyStore>()
        .AddAuthority((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            config.GetRequiredSection("blazorade:id").Bind(options);
        })
    ;

await builder.Build().RunAsync();

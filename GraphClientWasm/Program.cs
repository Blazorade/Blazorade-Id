using GraphClientWasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddBlazoradeId((sp, opt) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var authConfig = config.GetRequiredSection("blazorade:id");
        authConfig.Bind(opt);
    })
    .AddBlazoradeId("foo", (sp, opt) =>
    {
        opt.MetadataUri = "foo";
        opt.CacheMode = Blazorade.Id.Configuration.TokenCacheMode.Session;
    })
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

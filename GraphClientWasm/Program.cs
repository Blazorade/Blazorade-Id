using GraphClientWasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddBlazoradeAuthentication((sp, opt) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var authConfig = config.GetRequiredSection("blazorade:authentication");
        authConfig.Bind(opt);
    })
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

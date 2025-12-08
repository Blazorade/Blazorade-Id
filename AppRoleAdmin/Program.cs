using AppRoleAdmin;
using AppRoleAdmin.Services;
using Blazorade.Id.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddMudServices()
    .AddScoped<GraphClientService>()
    .AddBlazoradeIdWasmApplication()
    //.AddTokenStore<BlazorSessionTokenStore>()
    .AddTokenStore<BlazorPersistentTokenStore>()
    .AddPropertyStorage<BlazorPersistentPropertyStore>()
    .AddAuthority((sp, options) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        config.GetRequiredSection("blazorade:id").Bind(options);
    })
    
    ;

await builder.Build().RunAsync();

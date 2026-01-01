using Microsoft.Extensions.DependencyInjection;
using Blazorade.Id.Components.Pages;
using Blazorade.Id.Services;
using SimpleServerApp.Components;
using BlazoradeIdSampleComponents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    ;

builder.Services
    .AddBlazoradeIdServerApplication()
        .AddAuthority((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>(); 
            config.GetRequiredSection("blazorade:id").Bind(options);
            options.AuthorizationWindowWidth = 1280;
            options.AuthorizationWindowHeight = 768;
        })
        .AddTokenStore<BrowserSessionStorageTokenStore>()
        .AddRefreshTokenStore<BrowserSessionStorageRefreshTokenStore>()
    ;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(OAuthCallback).Assembly, typeof(SampleMenu).Assembly)
    .AddInteractiveServerRenderMode()
    ;

app.Run();

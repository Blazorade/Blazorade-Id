using Blazorade.Id.Components.Pages;
using Blazorade.Id.Services;
using SimpleServerApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    ;

builder.Services
    .AddBlazoradeCore()
    .AddBlazoradeIdServerApplication()
        .AddAuthority((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>(); 
            config.GetRequiredSection("blazorade:id").Bind(options);
        })


    // By default, Blazorade ID uses in-memory token storage, which means tokens are lost when the page is refreshed.
    // If you want to persist tokens across page refreshes, you can use one of the token stores below.
    //.AddTokenStore<BlazorSessionTokenStore>()
    .AddTokenStore<BlazorPersistentTokenStore>()
    .AddPropertyStore<BlazorPersistentPropertyStore>()
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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(OAuthCallback).Assembly)
    ;

app.Run();

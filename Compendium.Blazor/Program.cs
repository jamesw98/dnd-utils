using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Compendium.Blazor;
using Compendium.Blazor.Utils;
using Compendium.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddScoped<TokenUtil>();
builder.Services.AddScoped<ApiUtil>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AttackService>();
builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<MonsterService>();
builder.Services.AddMudServices();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
});

await builder.Build().RunAsync();
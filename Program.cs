using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using dnd_utils;
using dnd_utils.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<MonsterService>();
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<AttackService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<SpellService>();

await builder.Build().RunAsync();
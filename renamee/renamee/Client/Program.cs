using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using renamee.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddMudBlazorDialog();
builder.Services.AddMudBlazorSnackbar(config =>
{
    config.PositionClass = Defaults.Classes.Position.TopCenter;
    config.PreventDuplicates = false;
    config.NewestOnTop = false;
    config.ShowCloseIcon = false;
    config.VisibleStateDuration = 3000;
    config.HideTransitionDuration = 500;
    config.ShowTransitionDuration = 500;
    config.SnackbarVariant = Variant.Filled;
});
builder.Services.AddMudBlazorResizeListener();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

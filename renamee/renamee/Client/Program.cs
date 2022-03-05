using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using MudBlazor.Services;
using renamee.Client;
using renamee.Shared.Models;
using renamee.Shared.Services;
using renamee.Shared.Validators;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddValidatorsFromAssemblyContaining<JobValidator>(ServiceLifetime.Transient);

builder.Services.AddMudServices();
builder.Services.AddMudBlazorDialog();
builder.Services.AddMudBlazorSnackbar(config =>
{
    config.PositionClass = Defaults.Classes.Position.TopCenter;
    config.PreventDuplicates = false;
    config.NewestOnTop = false;
    config.ShowCloseIcon = false;
    config.VisibleStateDuration = 5000;
    config.HideTransitionDuration = 200;
    config.ShowTransitionDuration = 200;
    config.SnackbarVariant = Variant.Filled;
});
builder.Services.AddMudBlazorResizeListener();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<Client>(sp => new Client(builder.HostEnvironment.BaseAddress, sp.GetService<HttpClient>()));
builder.Services.AddTransient<IJob, Job>();
builder.Services.AddSingleton<JobsFactory>();

builder.Services.AddSingleton<HubConnection>(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
      .WithUrl(navigationManager.ToAbsoluteUri(renamee.Shared.Hubs.Consts.JobsHub))
      .WithAutomaticReconnect()
      .Build();
});

await builder.Build().RunAsync();


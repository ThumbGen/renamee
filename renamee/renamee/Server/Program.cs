using FluentValidation;
using renamee.Server.Options;
using renamee.Server.Repositories;
using renamee.Server.Services;
using renamee.Shared.Models;
using renamee.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices(services =>
{
    services.Configure<HostOptions>(hostOptions =>
    {
        hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5100); // to listen for incoming http connection on port 5100
    //options.ListenAnyIP(7001, configure => configure.UseHttps()); // to listen for incoming https connection on port 7001
});

// Add services to the container.

builder.Services.AddHealthChecks();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// services
builder.Services.AddValidatorsFromAssemblyContaining<JobOptionsValidator>();
builder.Services.AddTransient<IJob, Job>();
builder.Services.AddSingleton<IJobsRepository, JobsRepository>();
builder.Services.AddSingleton<IProcessorService, ProcessorService>();
builder.Services.AddHostedService<BackgroundProcessorService>();

// options
builder.Services.Configure<ProcessorOptions>(builder.Configuration.GetSection(ProcessorOptions.Processor));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseHealthChecks("/health");

app.Run();

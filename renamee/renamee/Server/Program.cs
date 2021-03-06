using FluentValidation;
using FluentValidation.AspNetCore;
using renamee.Server.Options;
using renamee.Server.Repositories;
using renamee.Server.Services;
using renamee.Shared.Hubs;
using renamee.Shared.Interfaces;
using renamee.Shared.Models;
using renamee.Shared.Services;
using renamee.Shared.Validators;
using Serilog;

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
builder.Services.AddControllersWithViews().AddFluentValidation();
builder.Services.AddRazorPages();

// signalR
builder.Services.AddSignalR();

// services
builder.Services.AddValidatorsFromAssemblyContaining<JobValidator>(ServiceLifetime.Transient);

builder.Services.AddTransient<IJob, Job>();
builder.Services.AddTransient<IRunnableJob, RunnableJob>();
builder.Services.AddSingleton<JobsFactory>();
builder.Services.AddSingleton<IJobsRepository, JobsRepository>();
builder.Services.AddSingleton<IJobsService, JobsService>();
builder.Services.AddSingleton<ISettingsRepository, SettingsRepository>();
builder.Services.AddSingleton<IProcessorService, ProcessorService>();
builder.Services.AddHostedService<BackgroundProcessorService>();
//builder.Services.AddSingleton<IReverseGeocoder, BigDataCloudService>();
builder.Services.AddSingleton<IReverseGeocoder, GeoNamesService>();

// open api
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "renamee API",
        Version = "v1"
    });

    c.EnableAnnotations();
});

// options
builder.Services.Configure<ProcessorOptions>(builder.Configuration.GetSection(ProcessorOptions.Processor));
builder.Services.Configure<RepositoryOptions>(builder.Configuration.GetSection(RepositoryOptions.Repository));

builder.Host.UseSerilog();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(app.Configuration)
    .CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

// open api
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "renamee API");
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<JobsHub>(Consts.JobsHub);

app.UseHealthChecks("/health");

app.Run();

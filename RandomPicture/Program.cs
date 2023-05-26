using RandomPicture.Options;
using RandomPicture.Services;
using Serilog;
using Serilog.Events;

#pragma warning disable ASP0004

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("RP_CONFIG_DIRECTORY") ?? "config";
var configFile = Path.GetFullPath(Path.Combine(configPath, "config.json"));

builder.Host.UseSerilog();

builder.Configuration.AddJsonFile(configFile, true, true);
builder.Services.AddOptions();
builder.Services.AddControllers();
builder.Services.AddSingleton<IProviderService, ProviderService>();

builder.Services.Configure<List<ProviderOptions>>(builder.Configuration.GetSection("Providers"));

var app = builder.Build();

app.MapControllers();

app.Run();

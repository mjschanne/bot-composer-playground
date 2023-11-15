using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StorageAccountAccessTest;

var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddJsonFile($"appsettings.{environment}.json", optional: true);
        config.AddUserSecrets<Program>();
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<AppSettings>(hostContext.Configuration.GetSection("StorageSettings"));
        services.AddHostedService<HostedService>();
    })
    .Build();



await host.RunAsync();

// this is all a little overkill but good practice with generic host :)
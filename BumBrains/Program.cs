using BumBrains;
using BumBrains.Extensions.ServiceCollection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration => 
    {
        configuration.AddJsonFile("local.settings.json");

        //TODO: Add environment-specific configuration!
        //configuration.AddJsonFile($"appsettings.{}.json");
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services
            .AddLink()
            .AddBankingClient(context.Configuration);

        services.AddHttpClient<CurrentBalance>(client =>
        {
            client.BaseAddress = new Uri("https://developer.bankofamerica.com");
        });
    })
    .Build();

await host.RunAsync();

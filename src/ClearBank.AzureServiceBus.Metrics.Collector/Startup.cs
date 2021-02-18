using ClearBank.AzureServiceBus.Metrics.Collector;
using ClearBank.AzureServiceBus.Metrics.Collector.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ClearBank.AzureServiceBus.Metrics.Collector
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //note: for testing locally either add a ServiceBus:ConnectionString environment variable 
            //or as an appsetting into your local.settings.json file.

            builder.Services
                   .Configure<TelemetryConfiguration>(
                        cfg => cfg.DefaultTelemetrySink
                          .TelemetryProcessorChainBuilder
                          .UseAdaptiveSampling(
                              maxTelemetryItemsPerSecond: 5,
                              excludedTypes: "Event;Exception",
                              includedTypes: "Dependency;Request;Trace")
                          .Build())
                   .AddSingleton<ServiceBusNamespaceService>()
                   .AddSingleton<SubscriptionMetricsTracker>()
                   .AddOptions<ServiceBusOptions>()
                    .Configure<IConfiguration>((opt, config) => config.GetSection("ServiceBus").Bind(opt));
        }
    }
}
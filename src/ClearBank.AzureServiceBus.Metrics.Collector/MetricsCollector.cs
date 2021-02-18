using System.Threading.Tasks;
using ClearBank.AzureServiceBus.Metrics.Collector.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ClearBank.AzureServiceBus.Metrics.Collector
{
    public class MetricsCollector
    {
        private readonly ServiceBusNamespaceService _namespaceService;
        private readonly SubscriptionMetricsTracker _metricsTracker;

        public MetricsCollector(
            ServiceBusNamespaceService namespaceService,
            SubscriptionMetricsTracker metricsTracker)
        {
            _namespaceService = namespaceService;
            _metricsTracker = metricsTracker;
        }

        [FunctionName("AzureServiceBus-MetricsCollector")]
        public async Task Run([TimerTrigger("%TimerInterval%")]TimerInfo myTimer, ILogger log)
        {
            var metrics = await _namespaceService.GetAllSubscriptionMetrics();

            _metricsTracker.Track(metrics);
        }
    }
}

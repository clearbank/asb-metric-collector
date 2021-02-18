using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Services
{
    public class ServiceBusNamespaceService
    {
        private readonly ManagementClient _mgmtClient;

        public ServiceBusNamespaceService(IOptions<ServiceBusOptions> svcBusOptions)
        {
            _mgmtClient = new ManagementClient(svcBusOptions.Value.ConnectionString);
        }

        public virtual async Task<IReadOnlyList<SubscriptionMetrics>> GetAllSubscriptionMetrics()
        {
            var allMetrics = new List<SubscriptionMetrics>();
            IList<TopicDescription> topics;

            const int batchSize = 10;
            var topicsRead = 0;

            do
            {

                topics = await _mgmtClient.GetTopicsAsync(batchSize, skip: topicsRead);
                topicsRead += topics.Count;

                // get subscription metrics
                var metricsTasks = topics.Select(t => _mgmtClient.GetSubscriptionsRuntimeInfoAsync(t.Path)).ToArray();
                await Task.WhenAll(metricsTasks);

                allMetrics.AddRange(
                    metricsTasks.SelectMany(t => t.Result)
                                .Select(s => new SubscriptionMetrics(s.TopicPath, s.SubscriptionName, s.MessageCountDetails.ActiveMessageCount, s.MessageCountDetails.DeadLetterMessageCount)));

            } while (topics.Any());

            return allMetrics;
        }
    }
}

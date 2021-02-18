using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Metrics;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Services
{
    public class SubscriptionMetricsTracker
    {
        private readonly TelemetryClient _telemetryClient;

        public SubscriptionMetricsTracker(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void Track(IReadOnlyList<SubscriptionMetrics> allMetrics)
        {
            if (!allMetrics.Any())
            {
                return;
            }

            foreach(var subscription in allMetrics)
            {
                _telemetryClient.TrackMetric(TrackedMetrics.ActiveMessages, subscription.ActiveMessagesCount,
                                    new Dictionary<string, string> { { TrackedMetrics.EntityNameDimension, subscription.EntityName } });

                _telemetryClient.TrackMetric(TrackedMetrics.DeadLetteredMessages, subscription.DeadLetteredMessagesCount,
                                    new Dictionary<string, string> { { TrackedMetrics.EntityNameDimension, subscription.EntityName } });
            }

            _telemetryClient.Flush();
        }
    }
}

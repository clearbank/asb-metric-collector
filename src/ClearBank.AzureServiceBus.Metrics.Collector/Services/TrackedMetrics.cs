using Microsoft.ApplicationInsights.Metrics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Services
{
    public class TrackedMetrics
    {
        public const string EntityNameDimension = "EntityName";
        public const string ActiveMessages = "ActiveMessages";
        public const string DeadLetteredMessages = "DeadLetteredMessages";
    }
}

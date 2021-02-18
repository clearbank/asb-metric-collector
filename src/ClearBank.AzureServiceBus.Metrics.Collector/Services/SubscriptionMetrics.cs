using Microsoft.Azure.ServiceBus;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Services
{
    public class SubscriptionMetrics
    {
        public SubscriptionMetrics(string topicName, string subscriptionName, long activeMessages, long deadLetteredMessages)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            ActiveMessagesCount = activeMessages;
            DeadLetteredMessagesCount = deadLetteredMessages;
        }

        public string TopicName { get; }

        public string SubscriptionName { get; }

        public string EntityName => EntityNameHelper.FormatSubscriptionPath(TopicName, SubscriptionName);

        public long ActiveMessagesCount { get; }

        public long DeadLetteredMessagesCount { get; }
    }
}

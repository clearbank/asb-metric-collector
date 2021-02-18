using ClearBank.AzureServiceBus.Metrics.Collector.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Tests.Component
{
    public class ServiceBusNamespaceServiceStub
    {
        private readonly Mock<ServiceBusNamespaceService> _servicebusNamespaceServiceMock;
        private readonly List<SubscriptionMetrics> _subscriptions;

        public ServiceBusNamespaceServiceStub()
        {
            var svcBusOptions = new OptionsWrapper<ServiceBusOptions>(
                                    new ServiceBusOptions
                                    {
                                        ConnectionString = "Endpoint=sb://notused.servicebus.windows.net/;SharedAccessKeyName=notused;SharedAccessKey=notused"
                                    });
            _servicebusNamespaceServiceMock = new Mock<ServiceBusNamespaceService>(() => new ServiceBusNamespaceService(svcBusOptions));
            _subscriptions = new List<SubscriptionMetrics>();

            _servicebusNamespaceServiceMock.Setup(m => m.GetAllSubscriptionMetrics())
                                           .ReturnsAsync(_subscriptions);
        }

        public SubscriptionMetrics SetupSubscription(string topic, string subscription, int activeMessages, int deadLetteredMessages)
        {
            var subMetrics = new SubscriptionMetrics(topic, subscription, activeMessages, deadLetteredMessages);

            _subscriptions.Add(subMetrics);

            return subMetrics;
        }

        public ServiceBusNamespaceService Service => _servicebusNamespaceServiceMock.Object;

        public void Reset()
        {
            _subscriptions.Clear();
        }
    }
}

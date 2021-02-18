using ClearBank.AzureServiceBus.Metrics.Collector.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Tests.Component
{
    public class MetricsCollectorShould : IDisposable
    {
        private readonly ServiceBusNamespaceServiceStub _servicebusNamespaceServiceStub;
        private readonly Mock<ITelemetryChannel> _telemetryChannelMock;
        private readonly MetricsCollector _metricsCollector;

        public MetricsCollectorShould()
        {
            _servicebusNamespaceServiceStub = new ServiceBusNamespaceServiceStub();
            _telemetryChannelMock = new Mock<ITelemetryChannel>();
            
            var telemetryConfiguration = new TelemetryConfiguration("");
            telemetryConfiguration.TelemetryChannel = _telemetryChannelMock.Object;

            var telemetryClient = new TelemetryClient(telemetryConfiguration);

            _metricsCollector = new MetricsCollector(
                _servicebusNamespaceServiceStub.Service,
                new SubscriptionMetricsTracker(telemetryClient));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public async Task Track_ActiveMessages_And_DeadLetteredMessages_Metrics_Per_Subscription(int numSubscriptions)
        {
            // arrange
            var random = new Random();
            var subscriptions = Enumerable.Range(0, numSubscriptions)
                                          .Select(n => _servicebusNamespaceServiceStub.SetupSubscription(
                                                         $"topic{n}",
                                                         $"subscription{n}",
                                                         random.Next(),
                                                         random.Next()))
                                          .ToList();

            // act
            await _metricsCollector.Run(null, NullLogger.Instance);

            // assert
            foreach(var subscription in subscriptions)
            {
                _telemetryChannelMock.Verify(
                    c => c.Send(It.Is<MetricTelemetry>(
                        m => m.Name == TrackedMetrics.ActiveMessages &&
                             m.Properties[nameof(subscription.EntityName)] == subscription.EntityName &&
                             m.Sum == subscription.ActiveMessagesCount)), Times.Once);

                _telemetryChannelMock.Verify(
                    c => c.Send(It.Is<MetricTelemetry>(
                        m => m.Name == TrackedMetrics.DeadLetteredMessages &&
                             m.Properties[nameof(subscription.EntityName)] == subscription.EntityName &&
                             m.Sum == subscription.DeadLetteredMessagesCount)), Times.Once);
            }

            _telemetryChannelMock.Verify(m => m.Flush(), Times.Once);
        }

        [Fact]
        public async Task Not_Push_Any_Metrics_When_No_Subscriptions_Available()
        {
            // act
            await _metricsCollector.Run(null, NullLogger.Instance);

            // assert
            _telemetryChannelMock.Verify(m => m.Send(It.IsAny<ITelemetry>()), Times.Never);
            _telemetryChannelMock.Verify(m => m.Flush(),Times.Never);
        }

        public void Dispose()
        {
            _servicebusNamespaceServiceStub.Reset();
        }
    }
}

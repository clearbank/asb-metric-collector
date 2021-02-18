using Microsoft.Extensions.Configuration;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Tests.Integration
{
    public class MetricsCollectorShould
    {
        private readonly FunctionAppClient _functionAppClient;

        public MetricsCollectorShould()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            _functionAppClient = new FunctionAppClient(
                                        config["function_name"],
                                        config["function_hostname"],
                                        config["function_master_key"]);
        }

        [Fact]
        public async Task Exist_And_Accept_Executions()
        {
            var response = await _functionAppClient.Run();
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }
    }
}

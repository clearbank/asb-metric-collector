using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.AzureServiceBus.Metrics.Collector.Tests.Integration
{
    public class FunctionAppClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _functionAppUri;

        public FunctionAppClient(string functionAppName, string functionHostName, string masterKey)
        {
            _functionAppUri = BuildUri(functionAppName, functionHostName);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-functions-key", masterKey);
        }

        public Task<HttpResponseMessage> Run()
            => _httpClient.PostAsync(
                _functionAppUri, 
                new StringContent("{}", Encoding.UTF8, "application/json"));

        private static Uri BuildUri(string functionAppName, string functionHostName)
            => new Uri($"https://{functionHostName}/admin/functions/{functionAppName}");
    }
}

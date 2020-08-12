using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.Core.Clients
{
    public class DeviceRegistryClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceRegistryClient> _logger;

        public DeviceRegistryClient(HttpClient httpClient, IConfiguration configuration, ILogger<DeviceRegistryClient> logger)
        {
            Guard.NotNull(httpClient, nameof(httpClient));
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(logger, nameof(logger));

            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task NotifyTwinChangedAsync(string deviceId, TwinInformation twinChangedNotification)
        {
            var baseUri = GetBaseUri();
            var uri = $"{baseUri}/api/v1/devices/{deviceId}/twin/notifications/changed";
            await PostRequestAsync(uri, twinChangedNotification);
        }

        public async Task SendMessageAsync(string deviceId, MessageTypes messageType, string rawMessage)
        {
            var baseUri = GetBaseUri();
            var uri = $"{baseUri}/api/v1/devices/{deviceId}/messages/{messageType}";
            await PostRequestAsync(uri, new Message{ Content = rawMessage });
        }

        private async Task PostRequestAsync(string uri, object body)
        {
            var rawNotification = JsonConvert.SerializeObject(body);

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(rawNotification, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private string GetBaseUri()
        {
            return _configuration["DeviceRegistry.API.BaseUri"];
        }
    }
}
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.Clients
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

        public async Task NotifyTwinChangedAsync(string deviceId, TwinChangedNotification twinChangedNotification)
        {
            var baseUri = _configuration["DeviceRegistry.API.BaseUri"];
            var uri = $"{baseUri}/api/v1/devices/{deviceId}/twin/notifications/changed";
            var rawNotification = JsonConvert.SerializeObject(twinChangedNotification);

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(rawNotification, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
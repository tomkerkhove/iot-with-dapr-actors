using System.Net.Http;
using GuardNet;
using Microsoft.Extensions.Logging;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.Clients
{
    public class DeviceRegistryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeviceRegistryClient> _logger;

        public DeviceRegistryClient(HttpClient httpClient, ILogger<DeviceRegistryClient> logger)
        {
            Guard.NotNull(httpClient, nameof(httpClient));
            Guard.NotNull(logger, nameof(logger));

            _httpClient = httpClient;
            _logger = logger;
        }
    }
}
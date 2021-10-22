using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomKerkhove.Dapr.APIs.Management.Exceptions;

namespace TomKerkhove.Dapr.APIs.Management.Repositories
{
    public class DeviceRegistryRepository
    {
        private readonly Dictionary<string, string> _inMemoryDeviceRegistry = new Dictionary<string, string>();

        public Task<string> GetDeviceIdAsync(string imei)
        {
            if (_inMemoryDeviceRegistry.ContainsKey(imei) == false)
            {
                throw new UnknownDeviceException(imei);
            }

            return Task.FromResult(_inMemoryDeviceRegistry[imei]);
        }

        public Task RegisterAsync(string deviceId, string imei)
        {
            _inMemoryDeviceRegistry[imei] = deviceId;
            return Task.CompletedTask;
        }

        public Task<List<string>> GetAllAsync()
        {
            return Task.FromResult(_inMemoryDeviceRegistry.Values.ToList());
        }
    }
}
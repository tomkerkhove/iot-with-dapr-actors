using System.Collections.Generic;
using System.Threading.Tasks;
using TomKerkhove.Dapr.APIs.Management.Exceptions;

namespace TomKerkhove.Dapr.APIs.Management.Repositories
{
    public class DeviceRegistryRepository
    {
        private readonly Dictionary<string, string> _inMemoryDeviceRegistry = new Dictionary<string, string>
        {
            {"ABC", "1"},
            {"DEF", "2"},
            {"GHI", "3"},
            {"JKL", "4"},
            {"MNO", "1"}
        };

        public Task<string> GetDeviceIdAsync(string imei)
        {
            if (_inMemoryDeviceRegistry.ContainsKey(imei) == false)
            {
                throw new UnknownDeviceException(imei);
            }

            return Task.FromResult(_inMemoryDeviceRegistry[imei]);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;

namespace TomKerkhove.Dapr.Actors.Device.Interface
{
    public interface IDeviceActor : IActor
    {
        Task ProvisionAsync(DeviceInfo info);
        Task SetInfoAsync(DeviceInfo info);
        Task<DeviceInfo> GetInfoAsync();
        Task ReceiveMessageAsync(MessageTypes type, string rawMessage);
        Task SetReportedPropertyAsync(Dictionary<string, string> reportedProperties);
    }
}

using System.Threading.Tasks;
using Dapr.Actors;
using Microsoft.Azure.Devices.Shared;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;

namespace TomKerkhove.Dapr.Actors.Device.Interface
{
    public interface IDeviceActor : IActor
    {
        Task ProvisionAsync(DeviceInfo info);
        Task SetInfoAsync(DeviceInfo info);
        Task<DeviceInfo> GetInfoAsync();
        Task ProcessMessageAsync(string rawMessage);
        Task SetTwinAsync(Twin twinInfo);
        Task SetReportedPropertyAsync(TwinCollection reportedProperties);
    }
}

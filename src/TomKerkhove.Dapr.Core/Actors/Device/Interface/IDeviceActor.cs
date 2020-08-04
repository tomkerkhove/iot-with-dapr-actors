using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using TomKerkhove.Dapr.Core.Actors.Device.Contracts;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.Core.Actors.Device.Interface
{
    public interface IDeviceActor : IActor
    {
        Task ProvisionAsync(DeviceInfo info);
        Task SetInfoAsync(DeviceInfo info);
        Task<DeviceInfo> GetInfoAsync();
        Task ReceiveMessageAsync(MessageTypes type, string rawMessage);
        Task TwinInformationChangedAsync(TwinChangedNotification notification);
        Task SetReportedPropertyAsync(Dictionary<string, string> reportedProperties);
    }
}

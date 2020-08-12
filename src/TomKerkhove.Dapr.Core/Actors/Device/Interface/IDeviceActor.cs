using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.Core.Actors.Device.Interface
{
    public interface IDeviceActor : IActor
    {
        Task ProvisionAsync(DeviceInfo info, TwinInformation initialTwinInfo);
        Task SetInfoAsync(DeviceInfo info);
        Task<DeviceInfo> GetInfoAsync();
        Task<Dictionary<string, string>> GetTagsAsync();
        Task ReceiveMessageAsync(MessageTypes type, Message message);
        Task NotifyTwinInformationChangedAsync(TwinInformation notification);
        Task SetReportedPropertyAsync(Dictionary<string, string> reportedProperties);
    }
}

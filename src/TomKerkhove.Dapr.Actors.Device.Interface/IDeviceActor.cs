using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace TomKerkhove.Dapr.Actors.Device.Interface
{
    public interface IDeviceActor : IActor
    {
        Task SetInfoAsync(DeviceState data);
        Task<DeviceState> GetInfoAsync();
        Task ReceiveMessageAsync(string rawMessage);
    }

    public class DeviceState
    {
        public string PropertyA { get; set; }
        public string PropertyB { get; set; }

        public override string ToString()
        {
            var propAValue = this.PropertyA == null ? "null" : this.PropertyA;
            var propBValue = this.PropertyB == null ? "null" : this.PropertyB;
            return $"PropertyA: {propAValue}, PropertyB: {propBValue}";
        }
    }
}

using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using TomKerkhove.Dapr.Actors.Device.Interface;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;

namespace TomKerkhove.Dapr.APIs.Management.Repositories
{
    public class DeviceRepository
    {
        public async Task<DeviceInfo> GetDataAsync(string deviceId)
        {
            var proxy = CreateActorProxy(deviceId);
            return await proxy.GetInfoAsync();
        }

        public async Task SetDataAsync(string deviceId, DeviceInfo newData)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.SetInfoAsync(newData);
        }

        public async Task SendMessageAsync(string deviceId, MessageTypes messageType, string payload)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.SendMessageAsync(messageType, payload);
        }

        private IDeviceActor CreateActorProxy(string deviceId)
        {
            var actorId = new ActorId(deviceId);

            // Create the local proxy by using the same interface that the service implements
            // By using this proxy, you can call strongly typed methods on the interface using Remoting.
            var proxy = ActorProxy.Create<IDeviceActor>(actorId, "DeviceActor");
            return proxy;
        }
    }
}
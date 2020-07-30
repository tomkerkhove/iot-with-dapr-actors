using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using TomKerkhove.Dapr.Actors.Device.Interface;

namespace TomKerkhove.Dapr.APIs.Management.Repositories
{
    public class DeviceRepository
    {
        public async Task<DeviceState> GetData(string deviceId)
        {
            var proxy = CreateActorProxy(deviceId);
            return await proxy.GetInfoAsync();
        }

        public async Task SetData(string deviceId, DeviceState newData)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.SetInfoAsync(newData);
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
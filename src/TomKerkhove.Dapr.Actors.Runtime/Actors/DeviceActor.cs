using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Azure.Devices.Shared;
using TomKerkhove.Dapr.Actors.Device.Interface;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;

namespace TomKerkhove.Dapr.Actors.Runtime.Actors
{
    internal class DeviceActor : Actor, IDeviceActor
    {
        private const string DeviceInfoKey = "device_info";
        private const string TwinStateKey = "twin_state";

        /// <summary>
        ///     Initializes a new instance of DeviceActor
        /// </summary>
        /// <param name="actorService">The Dapr.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Dapr.Actors.ActorId for this actor instance.</param>
        public DeviceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task ProvisionAsync(DeviceInfo info)
        {
            await SetInfoAsync(info);
            // TODO: Emit event
        }

        /// <summary>
        ///     Set DeviceInfo into actor's private state store
        /// </summary>
        /// <param name="data">the user-defined DeviceInfo which will be stored into state store as DeviceInfoKey state</param>
        public async Task SetInfoAsync(DeviceInfo data)
        {
            // Data is saved to configured state store implicitly after each method execution by Actor's runtime.
            // Data can also be saved explicitly by calling this.StateManager.SaveStateAsync();
            // State to be saved must be DataContract serializable.
            await StateManager.SetStateAsync(DeviceInfoKey, data);
        }

        /// <summary>
        ///     Set DeviceInfo into actor's private state store
        /// </summary>
        /// <param name="data">the user-defined DeviceInfo which will be stored into state store as DeviceInfoKey state</param>
        public async Task SetTwinAsync(Twin twinInfo)
        {
            // Data is saved to configured state store implicitly after each method execution by Actor's runtime.
            // Data can also be saved explicitly by calling this.StateManager.SaveStateAsync();
            // State to be saved must be DataContract serializable.
            await StateManager.SetStateAsync(TwinStateKey, twinInfo);
        }

        /// <summary>
        ///     Get DeviceInfo from actor's private state store
        /// </summary>
        /// <return>the user-defined DeviceInfo which is stored into state store as DeviceInfoKey state</return>
        public async Task<DeviceInfo> GetInfoAsync()
        {
            // Gets state from the state store.
            var potentialData = await StateManager.TryGetStateAsync<DeviceInfo>(DeviceInfoKey);
            return potentialData.HasValue ? potentialData.Value : null;
        }

        public Task ProcessMessageAsync(string rawMessage)
        {
            // TODO: Register timer to detect idle device 
            return Task.CompletedTask;
        }

        public Task SetReportedPropertyAsync(TwinCollection reportedProperties)
        {
            // TODO: Integrate with IoT Hub
            return Task.CompletedTask;
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {Id}");
            return Task.CompletedTask;
        }

        /// <summary>
        ///     This method is called whenever an actor is deactivated after a period of inactivity.
        /// </summary>
        protected override Task OnDeactivateAsync()
        {
            // Provides Opporunity to perform optional cleanup.
            Console.WriteLine($"Deactivating actor id: {Id}");
            return Task.CompletedTask;
        }
    }
}
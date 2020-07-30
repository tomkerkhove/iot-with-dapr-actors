using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using TomKerkhove.Dapr.Actors.Device.Interface;

namespace TomKerkhove.Dapr.Actors.Runtime.Actors
{
    internal class DeviceActor : Actor, IDeviceActor
    {
        private const string DeviceStateKey = "device_state";

        /// <summary>
        ///     Initializes a new instance of DeviceActor
        /// </summary>
        /// <param name="actorService">The Dapr.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Dapr.Actors.ActorId for this actor instance.</param>
        public DeviceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        ///     Set DeviceState into actor's private state store
        /// </summary>
        /// <param name="data">the user-defined DeviceState which will be stored into state store as DeviceStateKey state</param>
        public async Task SetInfoAsync(DeviceState data)
        {
            // Data is saved to configured state store implicitly after each method execution by Actor's runtime.
            // Data can also be saved explicitly by calling this.StateManager.SaveStateAsync();
            // State to be saved must be DataContract serializable.
            await StateManager.SetStateAsync(
                DeviceStateKey, // state name
                data); // data saved for the named state DeviceStateKey
        }

        /// <summary>
        ///     Get DeviceState from actor's private state store
        /// </summary>
        /// <return>the user-defined DeviceState which is stored into state store as DeviceStateKey state</return>
        public async Task<DeviceState> GetInfoAsync()
        {
            // Gets state from the state store.
            var potentialData = await StateManager.TryGetStateAsync<DeviceState>(DeviceStateKey);
            return potentialData.HasValue ? potentialData.Value : null;
        }

        public Task ReceiveMessageAsync(string rawMessage)
        {
            // TODO: Register timer to detect idle device 
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
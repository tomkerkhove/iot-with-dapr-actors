using System;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TomKerkhove.Dapr.Actors.Runtime
{
    public class ExtendedActor<TActor> : Actor
    {
        public ExtendedActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            Logger = Startup.Services.GetService<ILogger<TActor>>();
        }

        public IServiceProvider Services => Startup.Services;
        public ILogger Logger { get; }
    }
}
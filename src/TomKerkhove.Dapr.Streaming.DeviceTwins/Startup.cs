using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TomKerkhove.Dapr.Core.Clients;
using TomKerkhove.Dapr.Streaming.DeviceTwins;
using TomKerkhove.Dapr.Streaming.DeviceTwins.EventProcessing;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TomKerkhove.Dapr.Streaming.DeviceTwins
{
    public class Startup : FunctionsStartup
    {
        public static ServiceProvider Services;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<DeviceRegistryClient>();
            builder.Services.AddTransient<TwinChangedNotificationProcessor>();
            builder.Services.AddTransient<EventProcessor>();

            Services = builder.Services.BuildServiceProvider();
        }
    }
}
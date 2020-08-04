using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TomKerkhove.Dapr.DeviceTwin.Monitor;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Clients;
using TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TomKerkhove.Dapr.DeviceTwin.Monitor
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
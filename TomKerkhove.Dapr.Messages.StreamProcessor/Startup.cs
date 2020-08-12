using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TomKerkhove.Dapr.Core.Clients;
using TomKerkhove.Dapr.Messages.StreamProcessor;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TomKerkhove.Dapr.Messages.StreamProcessor
{
    public class Startup : FunctionsStartup
    {
        public static ServiceProvider Services;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<DeviceRegistryClient>();

            Services = builder.Services.BuildServiceProvider();
        }
    }
}
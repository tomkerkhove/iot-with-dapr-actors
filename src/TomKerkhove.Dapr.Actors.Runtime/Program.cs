using Dapr.Actors.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using TomKerkhove.Dapr.Actors.Runtime.Actors;

namespace TomKerkhove.Dapr.Actors.Runtime
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseActors(actorRuntime =>
                {
                    actorRuntime.RegisterActor<DeviceActor>();
                })
                .UseUrls("http://localhost:3000");
    }
}

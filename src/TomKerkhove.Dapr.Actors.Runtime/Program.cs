using System;
using Dapr.Actors.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TomKerkhove.Dapr.Actors.Runtime.Actors;

namespace TomKerkhove.Dapr.Actors.Runtime
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                CreateHostBuilder(args)
                    .Build()
                    .Run();

                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration configuration = CreateConfiguration(args);
            IHostBuilder webHostBuilder = CreateHostBuilder(args, configuration);

            return webHostBuilder;
        }

        private static IConfiguration CreateConfiguration(string[] args)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

            return configuration;
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            IHostBuilder webHostBuilder =
                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration(configBuilder => configBuilder.AddEnvironmentVariables())
                    .ConfigureSecretStore((config, secretStoreBuilder) =>
                    {
                        secretStoreBuilder.AddEnvironmentVariables();
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureKestrel(kestrelServerOptions => kestrelServerOptions.AddServerHeader = false)
                            .UseUrls("http://localhost:3000")
                            .UseActors(actorRuntime =>
                            {
                                actorRuntime.RegisterActor<DeviceActor>();
                            })
                            .UseSerilog()
                            .UseStartup<Startup>();
                    });

            return webHostBuilder;
        }
    }
}

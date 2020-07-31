using System;
using Arcus.Security.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing;

namespace TomKerkhove.Dapr.Actors.Runtime
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services.AddTransient<MessageProcessor>();

            Services = services.BuildServiceProvider();
        }

        public static ServiceProvider Services { get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            Log.Logger = CreateLoggerConfiguration(app.ApplicationServices).CreateLogger();
        }

        private LoggerConfiguration CreateLoggerConfiguration(IServiceProvider serviceProvider)
        {
            var secretProvider = serviceProvider.GetService<ISecretProvider>();
            var telemetryKey = secretProvider.GetRawSecretAsync("APPLICATION_INSIGHTS").Result;

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithVersion()
                .Enrich.WithComponentName("Actor Runtime")
                .WriteTo.Console()
                .WriteTo.AzureApplicationInsights(telemetryKey);
        }
    }
}

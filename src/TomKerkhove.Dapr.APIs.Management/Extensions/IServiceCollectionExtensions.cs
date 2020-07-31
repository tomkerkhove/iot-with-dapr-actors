using System;
using System.IO;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TomKerkhove.Dapr.APIs.Management.Repositories;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds dependencies
        /// </summary>
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<DeviceRepository>();
            services.AddTransient<DeviceRegistryRepository>();

            return services;
        }

        /// <summary>
        ///     Adds OpenAPI specs
        /// </summary>
        public static IServiceCollection AddOpenApiSpecs(this IServiceCollection services)
        {
            var openApiInformation = new OpenApiInfo
            {
                Title = "Device API",
                Version = "v1",
                Description = "API to manage all devices in our landscape."
            };

            services.AddSwaggerGen(swaggerGenerationOptions =>
            {
                swaggerGenerationOptions.SwaggerDoc("v1", openApiInformation);
                swaggerGenerationOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Docs", "TomKerkhove.Dapr.APIs.Management.Open-Api.xml"));

                swaggerGenerationOptions.OperationFilter<AddHeaderOperationFilter>("X-Transaction-Id", "Transaction ID is used to correlate multiple operation calls. A new transaction ID will be generated if not specified.", false);
                swaggerGenerationOptions.OperationFilter<AddResponseHeadersFilter>();
            });

            return services;
        }
    }
}
// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds OpenAPI UI
        /// </summary>
        public static IApplicationBuilder AddOpenApiUi(this IApplicationBuilder app)
        {
            app.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.RouteTemplate = "api/{documentName}/docs.json";
            });
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("/api/v1/docs.json", "Device API");
                swaggerUiOptions.RoutePrefix = "api/docs";
                swaggerUiOptions.DocumentTitle = "Device API";
            });

            return app;
        }
    }
}

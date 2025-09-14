using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Inventory.Management.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCustomSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(options =>
                {
                    options.DocumentTitle = "Inventory Management API";

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"Inventory Management API {description.GroupName}");
                    }

                    options.RoutePrefix = string.Empty;
                    options.DefaultModelsExpandDepth(-1);
                });
            }

            return app;
        }

        public static WebApplication UseCustomMiddleware(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Inventory.Management.Infra.Data;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Inventory.Management.Application;

namespace Inventory.Management.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder, string serviceName, string serviceVersion)
        {
            // Logs
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:4317");
                });
            });

            // Tracing
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName, serviceVersion))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri("http://localhost:4317");
                        });
                });

            return builder;
        }

        public static WebApplicationBuilder AddCustomSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var provider = builder.Services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = "Inventory Management API",
                        Version = description.ApiVersion.ToString(),
                        Description = "API for store chain inventory management.",
                        Contact = new OpenApiContact
                        {
                            Name = "Support",
                            Email = "support@inventory.com"
                        }
                    });
                }

                options.EnableAnnotations();

                // XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.OrderActionsBy(apiDesc => apiDesc.RelativePath);
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;
                    var actionApiVersionModel = apiDesc.ActionDescriptor.GetApiVersionModel();
                    if (actionApiVersionModel == null || !actionApiVersionModel.DeclaredApiVersions.Any())
                        return true;
                    return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
            });

            return builder;
        }

        public static WebApplicationBuilder AddApiVersioningAndExplorer(this WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            return builder;
        }

        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddPresentation()
                .AddApplication()
                .AddInfraDataSqliteInMemory()
                .AddControllers();

            return builder;
        }
    }
}

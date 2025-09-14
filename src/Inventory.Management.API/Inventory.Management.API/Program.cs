using Inventory.Management.API;
using Inventory.Management.Application;
using Inventory.Management.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Identidade do serviço
var serviceName = "Inventory.Management.API";
var serviceVersion = "1.0.0";

// Logs
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.AddOtlpExporter(o =>
    {
        o.Endpoint = new Uri("http://localhost:4317"); // OTLP Collector
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
                o.Endpoint = new Uri("http://localhost:4317"); // OTLP Collector
            });
    });

// Add services to the container.
builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfraDataSqliteInMemory()
    .AddControllers();

// Configuração do Versionamento
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

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    // Adiciona um documento do Swagger para cada versão da API
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(
            description.GroupName,
            new OpenApiInfo
            {
                Title = "Inventory Management API",
                Version = description.ApiVersion.ToString(),
                Description = "API para gerenciamento de inventário de rede de lojas",
                Contact = new OpenApiContact
                {
                    Name = "Suporte",
                    Email = "suporte@inventory.com"
                }
            });
    }

    // Configuração para documentação XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Ordena as ações por ordem alfabética
    options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

    // Configura o Swagger para usar o nome da versão da rota
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

        var actionApiVersionModel = apiDesc.ActionDescriptor.GetApiVersionModel();
        if (actionApiVersionModel == null || !actionApiVersionModel.DeclaredApiVersions.Any())
            return true;

        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        // Remove a versão anterior
        options.DocumentTitle = "Inventory Management API";

        // Configura um endpoint do Swagger para cada versão
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Inventory Management API {description.GroupName}");
        }

        options.RoutePrefix = string.Empty;
        options.DefaultModelsExpandDepth(-1); // Oculta a seção de esquemas
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Reflection;
using Inventory.Management.Application;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Inventory.Management.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPresentation()
    .AddApplication()
    .AddControllers();

// Configura��o do Versionamento
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

// Configura��o do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    // Adiciona um documento do Swagger para cada vers�o da API
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(
            description.GroupName,
            new OpenApiInfo
            {
                Title = "Inventory Management API",
                Version = description.ApiVersion.ToString(),
                Description = "API para gerenciamento de invent�rio de rede de lojas",
                Contact = new OpenApiContact
                {
                    Name = "Suporte",
                    Email = "suporte@inventory.com"
                }
            });
    }

    // Configura��o para documenta��o XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Ordena as a��es por ordem alfab�tica
    options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

    // Configura o Swagger para usar o nome da vers�o da rota
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
        // Remove a vers�o anterior
        options.DocumentTitle = "Inventory Management API";

        // Configura um endpoint do Swagger para cada vers�o
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Inventory Management API {description.GroupName}");
        }

        options.RoutePrefix = string.Empty;
        options.DefaultModelsExpandDepth(-1); // Oculta a se��o de esquemas
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

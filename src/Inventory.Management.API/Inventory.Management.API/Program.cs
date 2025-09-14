using Inventory.Management.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "Inventory.Management.API";
var serviceVersion = "1.0.0";

builder
    .AddOpenTelemetry(serviceName, serviceVersion)
    .AddApiVersioningAndExplorer()
    .AddCustomSwagger()
    .AddApplicationServices();

var app = builder.Build();

app
    .UseCustomSwagger()
    .UseCustomMiddleware();

app.MapHealthChecks("/health");

app.Run();

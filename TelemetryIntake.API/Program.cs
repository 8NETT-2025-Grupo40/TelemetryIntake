using TelemetryIntake.API.DependencyInjection;
using TelemetryIntake.API.Endpoints;
using TelemetryIntake.API.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.AddDocumentationServices();
builder.ConfigureDependencyInjectionServices();
builder.ConfigureHealthCheckServices();
builder.ConfigureLogging();

var app = builder.Build();

app.ConfigureDocumentation();

app.UseHttpsRedirection();

app.MapDataEndpoints();
app.MapHealthCheckEndpoints();

app.Run();
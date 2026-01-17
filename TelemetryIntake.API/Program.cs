using TelemetryIntake.API.DependencyInjection;
using TelemetryIntake.API.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.AddDocumentationServices();

var app = builder.Build();

app.ConfigureDocumentation();

app.UseHttpsRedirection();

app.Run();
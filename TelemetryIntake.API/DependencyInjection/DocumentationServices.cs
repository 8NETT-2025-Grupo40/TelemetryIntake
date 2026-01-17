namespace TelemetryIntake.API.DependencyInjection;

public static class DocumentationServices
{
	public static void AddDocumentationServices(this IHostApplicationBuilder applicationBuilder)
	{
		applicationBuilder.Services.AddOpenApi();
		applicationBuilder.Services.AddEndpointsApiExplorer();
		applicationBuilder.Services.AddSwaggerGen();
	}
}
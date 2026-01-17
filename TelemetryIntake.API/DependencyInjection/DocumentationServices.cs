using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

namespace TelemetryIntake.API.DependencyInjection;

public static class DocumentationServices
{
	public static void AddDocumentationServices(this IHostApplicationBuilder applicationBuilder)
	{
		applicationBuilder.Services.AddOpenApi();
		applicationBuilder.Services.AddEndpointsApiExplorer();
		applicationBuilder.Services.AddSwaggerGen(options =>
		{
			options.ExampleFilters();
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
			});

			options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
			{
				[new OpenApiSecuritySchemeReference("Bearer", document)] = []
			});
		});

		applicationBuilder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
	}
}
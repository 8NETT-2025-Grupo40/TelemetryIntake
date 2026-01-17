namespace TelemetryIntake.API.Setup;

public static class DocumentationSetup
{
	public static void ConfigureDocumentation(this WebApplication webApplication)
	{
		if (webApplication.Environment.IsDevelopment())
		{
			webApplication.MapOpenApi();
			webApplication.UseSwagger();
			webApplication.UseSwaggerUI();
		}
	}
}
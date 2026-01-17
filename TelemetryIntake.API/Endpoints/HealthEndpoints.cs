using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TelemetryIntake.API.Endpoints;

public static class HealthEndpoints
{
	public static void MapHealthCheckEndpoints(this WebApplication webApplication)
	{
		webApplication.MapHealthChecks(
			"/health",
			new HealthCheckOptions
			{
				ResultStatusCodes =
				{
					[HealthStatus.Healthy] = StatusCodes.Status200OK,
					[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
					[HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable
				},
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";

					var payload = new
					{
						Status = report.Status.ToString(),
						Checks = report.Entries.Select(entry => new
						{
							Name = entry.Key,
							Status = entry.Value.Status.ToString(),
							Description = entry.Value.Description,
							Error = entry.Value.Exception?.Message
						})
					};

					await context.Response.WriteAsJsonAsync(payload);
				}
			})
			.WithName("HealthCheck")
			.WithTags("Health Check")
			.WithSummary("This endpoint is used to check if the application is healthy.");
	}
}

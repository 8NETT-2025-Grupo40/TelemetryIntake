using Microsoft.AspNetCore.Mvc;
using TelemetryIntake.Domain.Entities;

namespace TelemetryIntake.API.Endpoints;

public static class TelemetryEndpoints
{
	public static void MapDataEndpoints(this WebApplication webApplication)
	{
		var telemetryGroup = webApplication.MapGroup("telemetry").WithTags("Telemetry");

		telemetryGroup
			.MapPut("data", ReceiveSensorData)
			.WithName("ReceiveSensorData")
			.WithSummary("Receive sensor data from farms.")
			.Produces(StatusCodes.Status204NoContent)
			.Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
	}

	private static IResult ReceiveSensorData([FromBody] SensorData sensorData)
	{
		try
		{
			return Results.NoContent();
		}
		catch (Exception e)
		{
			var problemDetails = new ProblemDetails
			{
				Title = e.Message,
				Status = StatusCodes.Status400BadRequest,
				Detail = e.ToString()
			};

			return Results.BadRequest(problemDetails);
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using TelemetryIntake.API.Swagger;
using TelemetryIntake.Domain.Entities;
using TelemetryIntake.Domain.Interfaces.Messaging;

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
			.Produces(StatusCodes.Status401Unauthorized)
			.Produces(StatusCodes.Status403Forbidden)
			.Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
			.WithMetadata(StatusCodes.Status400BadRequest, typeof(BadRequestExample))
			.RequireAuthorization("TelemetryWrite");
	}

	private static async ValueTask<IResult> ReceiveSensorData(
		[FromBody] SensorData sensorData,
		ITelemetryIngestionService telemetryIngestionService)
	{
		try
		{
			await telemetryIngestionService.SendSensorDataToQueueAsync(sensorData);
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

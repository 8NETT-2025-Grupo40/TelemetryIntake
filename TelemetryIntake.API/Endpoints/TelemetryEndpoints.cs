using Microsoft.AspNetCore.Mvc;
using TelemetryIntake.API.Sensor.Entities;
using TelemetryIntake.API.Swagger;
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

	public static async ValueTask<IResult> ReceiveSensorData(
		[FromBody] SensorReading sensorReading,
		ITelemetryIngestionService telemetryIngestionService)
	{
		try
		{
			await telemetryIngestionService.SendSensorDataToQueueAsync(sensorReading);
			return Results.NoContent();
		}
		catch (Exception e)
		{
			var problemDetails = new ProblemDetails
			{
				Title = e.Message,
				Status = StatusCodes.Status400BadRequest,
				Detail = e.ToString(),
				Type = string.Empty
			};

			return Results.BadRequest(problemDetails);
		}
	}
}

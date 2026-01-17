using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace TelemetryIntake.API.Swagger;

public class BadRequestExample : IExamplesProvider<ProblemDetails>
{
	public ProblemDetails GetExamples()
	{
		return new()
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "Error while receiving sensor data",
			Detail = "One or more sensor readings are missing or malformed."
		};
	}
}
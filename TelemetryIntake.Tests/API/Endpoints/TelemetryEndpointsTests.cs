using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TelemetryIntake.API.Endpoints;
using TelemetryIntake.Domain.Entities;
using TelemetryIntake.Domain.Interfaces.Messaging;

namespace TelemetryIntake.Tests.API.Endpoints;

public class TelemetryEndpointsTests
{
	private SensorData _sensorData = new SensorData
	{
		SensorId = Guid.Parse("2D964D65-15C2-4836-BAAA-D9D7B0CF80D0"),
		FarmId = Guid.Parse("49072487-10AE-4E48-8B10-50F076752D45"),
		FieldId = Guid.Parse("7395FBD7-074B-4A2D-B7C2-92A16D2BDEA6"),
		DateTime = DateTime.Now,
		AirTemperature = "20",
		AirHumidity = "50",
		SoilTemperature = "25",
		SoilHumidity = "75",
		RainMm = "10"
	};

	[Fact]
	public async Task OnReceiveSensorData_WhenDataIsReceivedCorrectly_ShouldInvokeSendSensorDataToQueueAsyncOnceAndReturnNoContent()
	{
		// Arrange
		const int expectedNumberOfCalls = 1;

		var telemetryIngestionServiceMock = Substitute.For<ITelemetryIngestionService>();

		// Act
		var response = await TelemetryEndpoints.ReceiveSensorData(_sensorData, telemetryIngestionServiceMock);

		// Assert
		await telemetryIngestionServiceMock.Received(expectedNumberOfCalls).SendSensorDataToQueueAsync(_sensorData);
		_ = Assert.IsType<NoContent>(response);
	}

	[Fact]
	public async Task OnReceiveSensorData_WhenExceptionIsThrown_ShouldInvokeSendSensorDataToQueueAsyncOnceAndReturnBadRequestWithExpectedValues()
	{
		// Arrange
		const int expectedNumberOfCalls = 1;
		const int expectedStatusCode = 400;

		const string exceptionMessage = "exceptionMessage";

		var telemetryIngestionServiceMock = Substitute.For<ITelemetryIngestionService>();

		telemetryIngestionServiceMock.When(x => x.SendSensorDataToQueueAsync(_sensorData)).Throw(x => new Exception(exceptionMessage));

		// Act
		var response = await TelemetryEndpoints.ReceiveSensorData(_sensorData, telemetryIngestionServiceMock);

		// Assert
		await telemetryIngestionServiceMock.Received(expectedNumberOfCalls).SendSensorDataToQueueAsync(_sensorData);
		var convertedResponse = Assert.IsType<BadRequest<ProblemDetails>>(response);

		Assert.Equal(exceptionMessage, convertedResponse?.Value?.Title);
		Assert.Equal(expectedStatusCode, convertedResponse?.Value?.Status);
		Assert.NotNull(convertedResponse?.Value?.Detail);
		Assert.NotEqual(string.Empty, convertedResponse?.Value?.Detail);
	}
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TelemetryIntake.API.Endpoints;
using TelemetryIntake.API.Sensor.Entities;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Tests.API.Endpoints;

public class TelemetryEndpointsTests
{
	private static readonly Guid _sensorId = Guid.Parse("2D964D65-15C2-4836-BAAA-D9D7B0CF80D0");
	private static readonly Guid _farmId = Guid.Parse("49072487-10AE-4E48-8B10-50F076752D45");
	private static readonly Guid _fieldId = Guid.Parse("7395FBD7-074B-4A2D-B7C2-92A16D2BDEA6");
	private static readonly double _airTemperature = 20;
	private static readonly double _airHumidity = 50;
	private static readonly double _soilTemperature = 25;
	private static readonly double _soilHumidity = 75;
	private static readonly double _rainMm = 10;

	private readonly SensorReading _sensorReading = new()
	{
		SensorId = _sensorId,
		FarmId = _farmId,
		FieldId = _fieldId,
		Timestamp = DateTime.Now,
		AirTemperature = _airTemperature,
		AirHumidity = _airHumidity,
		SoilTemperature = _soilTemperature,
		SoilHumidity = _soilHumidity,
		RainMm = _rainMm
	};

	[Fact]
	public async Task OnReceiveSensorData_WhenDataIsReceivedCorrectly_ShouldInvokeSendSensorDataToQueueAsyncOnceAndReturnNoContent()
	{
		// Arrange
		const int expectedNumberOfCalls = 1;

		var telemetryIngestionServiceMock = Substitute.For<ITelemetryIngestionService>();

		// Act
		var response = await TelemetryEndpoints.ReceiveSensorData(_sensorReading, telemetryIngestionServiceMock);

		var expectedSensorData = Arg.Is<SensorData>(x =>
			x.SensorId == _sensorId &&
			x.FarmId == _farmId &&
			x.FieldId == _fieldId &&
			x.AirTemperature == _airTemperature &&
			x.AirHumidity == _airHumidity &&
			x.SoilTemperature == _soilTemperature &&
			x.SoilHumidity == _soilHumidity &&
			x.RainMm == _rainMm);

		// Assert
		await telemetryIngestionServiceMock.Received(expectedNumberOfCalls).SendSensorDataToQueueAsync(expectedSensorData);

		_ = Assert.IsType<Accepted>(response);
	}

	[Fact]
	public async Task OnReceiveSensorData_WhenExceptionIsThrown_ShouldInvokeSendSensorDataToQueueAsyncOnceAndReturnBadRequestWithExpectedValues()
	{
		// Arrange
		const int expectedNumberOfCalls = 1;
		const int expectedStatusCode = 400;

		const string exceptionMessage = "exceptionMessage";

		var telemetryIngestionServiceMock = Substitute.For<ITelemetryIngestionService>();

		var expectedSensorData = Arg.Is<SensorData>(x =>
			x.SensorId == _sensorId &&
			x.FarmId == _farmId &&
			x.FieldId == _fieldId &&
			x.AirTemperature == _airTemperature &&
			x.AirHumidity == _airHumidity &&
			x.SoilTemperature == _soilTemperature &&
			x.SoilHumidity == _soilHumidity &&
			x.RainMm == _rainMm);

		telemetryIngestionServiceMock.When(x => x.SendSensorDataToQueueAsync(expectedSensorData)).Throw(x => new Exception(exceptionMessage));

		// Act
		var response = await TelemetryEndpoints.ReceiveSensorData(_sensorReading, telemetryIngestionServiceMock);

		// Assert
		await telemetryIngestionServiceMock.Received(expectedNumberOfCalls).SendSensorDataToQueueAsync(Arg.Any<SensorData>());

		var convertedResponse = Assert.IsType<BadRequest<ProblemDetails>>(response);

		Assert.Equal(exceptionMessage, convertedResponse?.Value?.Title);
		Assert.Equal(expectedStatusCode, convertedResponse?.Value?.Status);
		Assert.NotNull(convertedResponse?.Value?.Detail);
		Assert.NotEqual(string.Empty, convertedResponse?.Value?.Detail);
	}
}
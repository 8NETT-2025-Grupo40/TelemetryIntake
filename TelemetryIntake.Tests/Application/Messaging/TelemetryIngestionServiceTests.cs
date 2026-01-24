using NSubstitute;
using TelemetryIntake.Application.Messaging;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Tests.Application.Messaging;

public class TelemetryIngestionServiceTests
{
	private readonly ITelemetryPublisher _telemetryPublisherMock;
	
	private readonly TelemetryIngestionService _serviceUnderTest;

	public TelemetryIngestionServiceTests()
	{
		_telemetryPublisherMock = Substitute.For<ITelemetryPublisher>();
		_serviceUnderTest = new TelemetryIngestionService(_telemetryPublisherMock);
	}

	[Fact]
	public async Task OnSendSensorDataToQueueAsync_WhenDataIsReceived_ShouldInvokeEnqueueSensorDataAsyncOnce()
	{
		// Arrange
		const int expectedNumberOfCalls = 1;

		var sensorData = new SensorData(
								  Guid.Parse("2D964D65-15C2-4836-BAAA-D9D7B0CF80D0"),
								  Guid.Parse("49072487-10AE-4E48-8B10-50F076752D45"),
								  Guid.Parse("7395FBD7-074B-4A2D-B7C2-92A16D2BDEA6"),
								  DateTime.Now,
								  20,
								  50,
								  25,
								  75,
								  10);

		// Act
		await _serviceUnderTest.SendSensorDataToQueueAsync(sensorData);

		// Assert
		await _telemetryPublisherMock.Received(expectedNumberOfCalls).EnqueueSensorDataAsync(sensorData);
	}
}
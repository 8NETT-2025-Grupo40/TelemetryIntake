using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Application.Messaging;

public class TelemetryIngestionService(ITelemetryPublisher telemetryPublisher) : ITelemetryIngestionService
{
	public async ValueTask SendSensorDataToQueueAsync(SensorData sensorData)
	{
		await telemetryPublisher.EnqueueSensorDataAsync(sensorData);
	}
}
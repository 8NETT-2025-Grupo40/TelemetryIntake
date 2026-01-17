using TelemetryIntake.Domain.Entities;
using TelemetryIntake.Domain.Interfaces.Messaging;

namespace TelemetryIntake.Application.Messaging;

public class TelemetryIngestionService(ITelemetryPublisher telemetryPublisher) : ITelemetryIngestionService
{
	public async ValueTask SendSensorDataToQueueAsync(SensorData sensorData)
	{
		await telemetryPublisher.EnqueueSensorDataAsync(sensorData);
	}
}
using TelemetryIntake.Domain.Entities;

namespace TelemetryIntake.Domain.Interfaces.Messaging;

public interface ITelemetryIngestionService
{
	ValueTask SendSensorDataToQueueAsync(SensorData sensorData);
}
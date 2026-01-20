using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Domain.Interfaces.Messaging;

public interface ITelemetryIngestionService
{
	ValueTask SendSensorDataToQueueAsync(SensorData sensorData);
}
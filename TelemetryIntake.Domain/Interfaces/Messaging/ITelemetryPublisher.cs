using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Domain.Interfaces.Messaging;

public interface ITelemetryPublisher
{
	ValueTask EnqueueSensorDataAsync(SensorData sensorData);
}
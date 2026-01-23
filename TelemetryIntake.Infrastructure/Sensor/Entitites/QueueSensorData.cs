using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Infrastructure.Sensor.Entitites;

internal class QueueSensorData(string queueName, SensorData sensorData)
{
	public string QueueName { get; set; } = queueName;
	public SensorData SensorData { get; set; } = sensorData;
}
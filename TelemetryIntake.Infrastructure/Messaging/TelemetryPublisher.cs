using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Infrastructure.Messaging;

public class TelemetryPublisher : ITelemetryPublisher
{
	public async ValueTask EnqueueSensorDataAsync(SensorData sensorData)
	{
		// Simulate communication with AWS
		await Task.Delay(TimeSpan.FromSeconds(5));
	}
}
namespace TelemetryIntake.Domain.Sensor.Entities;

public class SensorData(
	Guid sensorId,
	Guid farmId,
	Guid fieldId,
	DateTimeOffset dateTime,
	double airTemperature,
	double airHumidity,
	double soilTemperature,
	double soilHumidity,
	double rainMm)
{
	public Guid ReadingId { get; set; } = Guid.NewGuid();
	public Guid SensorId { get; set; } = sensorId;
	public Guid FarmId { get; set; } = farmId;
	public Guid FieldId { get; set; } = fieldId;
	public DateTimeOffset Timestamp { get; set; } = dateTime;
	public double AirTemperature { get; set; } = airTemperature;
	public double AirHumidity { get; set; } = airHumidity;
	public double SoilTemperature { get; set; } = soilTemperature;
	public double SoilHumidity { get; set; } = soilHumidity;
	public double RainMm { get; set; } = rainMm;
}
using System.Text.Json.Serialization;

namespace TelemetryIntake.Domain.Sensor.Entities;

public class SensorData(
	Guid sensorId,
	Guid farmId,
	Guid fieldId,
	DateTime dateTime,
	string? airTemperature,
	string? airHumidity,
	string? soilTemperature,
	string? soilHumidity,
	string? rainMm)
{
	public Guid ReadId { get; set; } = Guid.NewGuid();
	public Guid SensorId { get; set; } = sensorId;
	public Guid FarmId { get; set; } = farmId;
	public Guid FieldId { get; set; } = fieldId;
	public DateTime DateTime { get; set; } = dateTime;
	public string? AirTemperature { get; set; } = airTemperature;
	public string? AirHumidity { get; set; } = airHumidity;
	public string? SoilTemperature { get; set; } = soilTemperature;
	public string? SoilHumidity { get; set; } = soilHumidity;
	public string? RainMm { get; set; } = rainMm;
}
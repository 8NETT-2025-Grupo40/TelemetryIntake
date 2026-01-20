using System.Text.Json.Serialization;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.API.Sensor.Entities;

public class SensorReading
{
	[JsonPropertyName("SensorId")]
	public Guid SensorId { get; set; }

	[JsonPropertyName("FarmId")]
	public Guid FarmId { get; set; }

	[JsonPropertyName("FieldId")]
	public Guid FieldId { get; set; }

	[JsonPropertyName("DateTime")]
	public DateTime DateTime { get; set; }

	[JsonPropertyName("AirTemperature")]
	public string? AirTemperature { get; set; }

	[JsonPropertyName("AirHumidity")]
	public string? AirHumidity { get; set; }

	[JsonPropertyName("SoilTemperature")]
	public string? SoilTemperature { get; set; }

	[JsonPropertyName("SoilHumidity")]
	public string? SoilHumidity { get; set; }

	[JsonPropertyName("RainMm")]
	public string? RainMm { get; set; }

	public static implicit operator SensorData(SensorReading sensorReading) => 
		new(sensorReading.SensorId,
			sensorReading.FarmId,
			sensorReading.FieldId,
			sensorReading.DateTime,
			sensorReading.AirTemperature,
			sensorReading.AirHumidity,
			sensorReading.SoilTemperature,
			sensorReading.SoilHumidity,
			sensorReading.RainMm);
}

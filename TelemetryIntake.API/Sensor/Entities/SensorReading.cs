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
	public DateTimeOffset Timestamp { get; set; }

	[JsonPropertyName("AirTemperature")]
	public double AirTemperature { get; set; }

	[JsonPropertyName("AirHumidity")]
	public double AirHumidity { get; set; }

	[JsonPropertyName("SoilTemperature")]
	public double SoilTemperature { get; set; }

	[JsonPropertyName("SoilHumidity")]
	public double SoilHumidity { get; set; }

	[JsonPropertyName("RainMm")]
	public double RainMm { get; set; }

	public static implicit operator SensorData(SensorReading sensorReading) => 
		new(sensorReading.SensorId,
			sensorReading.FarmId,
			sensorReading.FieldId,
			sensorReading.Timestamp,
			sensorReading.AirTemperature,
			sensorReading.AirHumidity,
			sensorReading.SoilTemperature,
			sensorReading.SoilHumidity,
			sensorReading.RainMm);
}

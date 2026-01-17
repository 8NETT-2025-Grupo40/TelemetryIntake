namespace TelemetryIntake.Domain.Entities;

public class SensorData
{
	public Guid SensorId { get; set; }
	public Guid FarmId { get; set; }
	public Guid FieldId { get; set; }
	public DateTime DateTime { get; set; }
	public string? AirTemperature { get; set; }
	public string? AirHumidity { get; set; }
	public string? SoilTemperature { get; set; }
	public string? SoilHumidity { get; set; }
	public string? RainMm { get; set; }
}
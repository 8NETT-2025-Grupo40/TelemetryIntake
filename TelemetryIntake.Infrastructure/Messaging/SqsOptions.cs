namespace TelemetryIntake.Infrastructure.Messaging;

public class SqsOptions
{
	public string? QueueUrl { get; set; }
	public string? Region { get; set; }
}
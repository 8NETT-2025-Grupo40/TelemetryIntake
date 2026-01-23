namespace TelemetryIntake.Infrastructure.Messaging;

public class SqsOptions
{
	public string? QueueUrl { get; set; }
	public string? QueueName { get; set; }
}

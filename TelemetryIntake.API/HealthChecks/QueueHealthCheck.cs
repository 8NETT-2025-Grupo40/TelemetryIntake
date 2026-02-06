using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TelemetryIntake.Application.Results;
using TelemetryIntake.Infrastructure.Messaging;

namespace TelemetryIntake.API.HealthChecks;

public class QueueHealthCheck : IHealthCheck
{
	private readonly IOptions<SqsOptions> _sqsOptions;
	private readonly IAmazonSQS _amazonSQSClient;
	private readonly ILogger<QueueHealthCheck> _logger;

	public QueueHealthCheck(
		IOptions<SqsOptions> sqsOptions,
		IAmazonSQS amazonSQSClient,
		ILogger<QueueHealthCheck> logger)
	{
		_sqsOptions = sqsOptions;
		_amazonSQSClient = amazonSQSClient;
		_logger = logger;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		return HealthCheckResult.Healthy("Queue is ready to receive messages.");

		var healthyResult = await ValidateQueue();

		if (healthyResult.IsSuccessful)
		{
			return HealthCheckResult.Healthy("Queue is ready to receive messages.");
		}

		return HealthCheckResult.Unhealthy(healthyResult.Message);
	}

	private async ValueTask<Result> ValidateQueue()
	{
		try
		{
			var request = new GetQueueAttributesRequest
			{
				QueueUrl = _sqsOptions.Value.QueueUrl,
				AttributeNames = [QueueAttributeName.QueueArn]
			};

			 _ = await _amazonSQSClient.GetQueueAttributesAsync(request);
			return Result.Success();
		}
		catch (Exception e)
		{
			const string message = "There was an error on the Queue Health Check";
			_logger.LogError(e, message);

			var errorMessage = $"{message}\n\n{e}";

			return Result.Error(errorMessage);
		}
	}
}
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using TelemetryIntake.Application.Observability;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Infrastructure.Messaging;

public class TelemetryPublisher : ITelemetryPublisher
{
	private readonly IAmazonSQS _client;
	private readonly ILogger<TelemetryPublisher> _logger;
	private readonly IOptions<SqsOptions> _sqsOptions;

	public TelemetryPublisher(ILogger<TelemetryPublisher> logger, IOptions<SqsOptions> sqsOptions, IAmazonSQS client)
	{
		_logger = logger;
		_sqsOptions = sqsOptions;
		_client = client;
	}

	public async ValueTask EnqueueSensorDataAsync(SensorData sensorData)
	{
		var activity = TelemetryIntakeObservabilityHandler.StartActivity(TelemetryIntakeObservabilityHandler.ActivitySourceName);

		try
		{
			var queueUrl = _sqsOptions.Value.QueueUrl;

			if (string.IsNullOrWhiteSpace(queueUrl))
			{
				throw new Exception("Queue url is empty - Data not sent");
			}

			TelemetryIntakeObservabilityHandler.SetSqsContext(activity, sensorData.FieldId, sensorData.FarmId, sensorData.SensorId, queueUrl);

			TelemetryIntakeObservabilityHandler.MarkProcessing(activity);

			await SendMessage(JsonSerializer.Serialize(sensorData), queueUrl);

			TelemetryIntakeObservabilityHandler.MarkSuccess(activity);
		}
		catch (Exception e)
		{
			_logger.LogError("Could not send message to queue\n{Message}", e.Message);

			TelemetryIntakeObservabilityHandler.RecordException(activity, e);
			
			TelemetryIntakeObservabilityHandler.MarkFailure(activity, e.Message);

			throw;
		}
	}

	private async ValueTask SendMessage(string jsonMessage, string queueUrl)
	{
		var sendMessageRequest = new SendMessageRequest
		{
			MessageBody = jsonMessage,
			QueueUrl = queueUrl,
			MessageGroupId = Guid.NewGuid().ToString(),
			MessageDeduplicationId = Guid.NewGuid().ToString()
		};

		_ = await _client.SendMessageAsync(sendMessageRequest);
	}
}
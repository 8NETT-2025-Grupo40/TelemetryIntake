using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Domain.Sensor.Entities;

namespace TelemetryIntake.Infrastructure.Messaging;

public class TelemetryPublisher : ITelemetryPublisher
{
	private readonly RegionEndpoint ServiceRegion = RegionEndpoint.USEast1;
	private readonly AmazonSQSClient _client;
	private readonly ILogger<TelemetryPublisher> _logger;
	private readonly IOptions<SqsOptions> _options;

	public TelemetryPublisher(ILogger<TelemetryPublisher> logger, IOptions<SqsOptions> options)
	{
		_client = new AmazonSQSClient(ServiceRegion);
		_logger = logger;
		_options = options;
	}

	public async ValueTask EnqueueSensorDataAsync(SensorData sensorData)
	{
		var queueName = _options.Value.QueueName;

		if (string.IsNullOrWhiteSpace(queueName))
		{
			throw new Exception("Queue name is empty - Data not sent");
		}

		await SendMessage(JsonSerializer.Serialize(sensorData));
	}

	private async ValueTask SendMessage(string jsonMessage)
	{
		var queueUrl = _options.Value.QueueUrl;

		if (string.IsNullOrWhiteSpace(queueUrl))
		{
			throw new Exception("Queue url is empty - Data not sent");
		}

		var sendMessageRequest = new SendMessageRequest
		{
			MessageBody = jsonMessage,
			QueueUrl = queueUrl,
			MessageGroupId = Guid.NewGuid().ToString(),
			MessageDeduplicationId = Guid.NewGuid().ToString()
		};

		try
		{
			var response = await _client.SendMessageAsync(sendMessageRequest);
		}
		catch (Exception e)
		{
			_logger.LogError("Could not send message to queue\n{Message}", e.Message);
			throw;
		}
	}
}
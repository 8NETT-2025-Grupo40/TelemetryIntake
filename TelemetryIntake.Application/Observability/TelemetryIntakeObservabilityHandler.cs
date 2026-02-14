using System.Diagnostics;

namespace TelemetryIntake.Application.Observability;

public static class TelemetryIntakeObservabilityHandler
{
	public const string ActivitySourceName = "TelemetryIntake";
	public const string ProcessingStatusSuccess = "success";
	
	public const string AttributeFarmId = "telemtry-intake.farm.id";
	public const string AttributeFieldId = "telemetry-intake.field.id";

	private const string AttributeMessagingDestinationName = "messaging.destination.name";
	private const string AttributeMessagingMessageId = "messaging.message.id";

	private const string AttributeProcessingStatus = "telemetry-intake.processing.status";

	public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
	
	public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal) => ActivitySource.StartActivity(name, kind);

	public static void SetSqsContext(
		Activity? activity,
		Guid? fieldId,
		Guid? farmId,
		Guid? sensorId,
		string ? queueUrl)
	{
		if (activity is null)
		{
			return;
		}

		if (fieldId == Guid.Empty)
		{
			activity.SetTag(AttributeFieldId, fieldId);
		}


		if (farmId == Guid.Empty)
		{
			activity.SetTag(AttributeFarmId, farmId);
		}

		if (sensorId == Guid.Empty)
		{
			activity.SetTag(AttributeMessagingMessageId, sensorId);
		}

		if (!string.IsNullOrWhiteSpace(queueUrl))
		{
			activity.SetTag(AttributeMessagingDestinationName, ExtractQueueName(queueUrl));
		}
	}

	public static void MarkSuccess(Activity? activity)
	{
		if (activity is null)
		{
			return;
		}

		activity.SetStatus(ActivityStatusCode.Ok);
	}

	public static void MarkProcessing(Activity? activity)
	{
		if (activity is null)
		{
			return;
		}

		activity.SetTag(AttributeProcessingStatus, ProcessingStatusSuccess);
	}

	public static void MarkFailure(Activity? activity, string reason)
	{
		if (activity is null)
		{
			return;
		}

		activity.SetStatus(ActivityStatusCode.Error, reason);
	}

	public static void RecordException(Activity? activity, Exception exception)
	{
		if (activity is null)
		{
			return;
		}

		activity.AddEvent(new ActivityEvent(
			"exception",
			tags: new ActivityTagsCollection
			{
				{ "exception.type", exception.GetType().FullName },
				{ "exception.message", exception.Message },
				{ "exception.stacktrace", exception.StackTrace }
			}));
	}

	private static string ExtractQueueName(string queueUrl)
	{
		var separatorIndex = queueUrl.LastIndexOf('/');

		return separatorIndex >= 0 && separatorIndex < queueUrl.Length - 1
			? queueUrl[(separatorIndex + 1)..]
			: queueUrl;
	}
}

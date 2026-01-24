namespace TelemetryIntake.Application.Results;

public class Result
{
	private Result(string? message, bool isSuccessful)
	{
		Message = message;
		IsSuccessful = isSuccessful;
	}

	public string? Message { get; set; }
	public bool IsSuccessful { get; set; }

	public static Result Success() =>
		new("Success", true);

	public static Result Error(string message) => new(message, false);
}

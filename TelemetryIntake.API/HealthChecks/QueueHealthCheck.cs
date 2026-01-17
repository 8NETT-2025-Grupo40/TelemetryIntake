using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TelemetryIntake.API.HealthChecks;

public class QueueHealthCheck : IHealthCheck
{
	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		// TODO: Modificar isso para que realmente seja uma validação de saúde por meio da fila SQS.
		return Task.FromResult(HealthCheckResult.Healthy());
	}
}
using TelemetryIntake.Application.Messaging;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Infrastructure.Messaging;

namespace TelemetryIntake.API.DependencyInjection;

public static class ServiceConfiguration
{
	public static void ConfigureDependencyInjectionServices(this IHostApplicationBuilder applicationBuilder)
	{
		applicationBuilder.Services.AddTransient<ITelemetryIngestionService, TelemetryIngestionService>();
		applicationBuilder.Services.AddTransient<ITelemetryPublisher, TelemetryPublisher>();
	}
}
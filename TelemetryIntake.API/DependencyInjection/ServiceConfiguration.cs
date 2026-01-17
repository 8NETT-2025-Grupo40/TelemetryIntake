using Serilog;
using Serilog.Formatting.Compact;
using TelemetryIntake.API.HealthChecks;
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

	public static void ConfigureHealthCheckServices(this IHostApplicationBuilder applicationBuilder)
	{
		applicationBuilder.Services.AddHealthChecks().AddCheck<QueueHealthCheck>("QueueHealthCheck");
	}

	public static void ConfigureLogging(this IHostApplicationBuilder applicationBuilder)
	{
		var loggerConfiguration = new LoggerConfiguration();

		if (applicationBuilder.Environment.IsDevelopment())
		{
			loggerConfiguration.WriteTo.Console();
		}
		else
		{
			loggerConfiguration
				.Enrich.FromLogContext()
				.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
		}

		Log.Logger = loggerConfiguration.CreateLogger();

		applicationBuilder.Services.AddSerilog();
	}
}
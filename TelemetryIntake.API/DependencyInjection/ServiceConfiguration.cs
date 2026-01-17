using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Formatting.Compact;
using TelemetryIntake.API.HealthChecks;
using TelemetryIntake.Application.Messaging;
using TelemetryIntake.Domain.Interfaces.Messaging;
using TelemetryIntake.Infrastructure.Messaging;

namespace TelemetryIntake.API.DependencyInjection;

public static class ServiceConfiguration
{
	private const string M2MScheme = "CognitoM2M";

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

	public static void ConfigureCognitoM2MAuthentication(
		this IHostApplicationBuilder applicationBuilder)
	{
		// Lê configurações do Cognito do appsettings.json
		var configuration = applicationBuilder.Configuration;
		var region = configuration["COGNITO_REGION"];
		var userPoolId = configuration["COGNITO_USER_POOL_ID"];
		var m2mClientId = configuration["COGNITO_M2M_CLIENT_ID"]; // Client ID específico para M2M!

		// Mesmo User Pool dos usuários, mas App Client diferente
		var authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";

		// Registra um scheme de autenticação SEPARADO para M2M
		// Isso permite ter endpoints que aceitam só usuário, só M2M, ou ambos
		applicationBuilder.
			Services
			.AddAuthentication()
			.AddJwtBearer(M2MScheme, options =>
			{
				options.Authority = authority;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = authority,
					ValidateAudience = false,
					ValidateLifetime = true,
					RequireExpirationTime = true,
					RequireSignedTokens = true,
					ClockSkew = TimeSpan.FromMinutes(2)
				};

				// Validações customizadas para tokens M2M
				options.Events = new JwtBearerEvents
				{
					OnTokenValidated = ctx =>
					{
						// Tokens M2M também são access tokens
						var tokenUse = ctx.Principal?.FindFirst("token_use")?.Value;
						if (!string.Equals(tokenUse, "access", StringComparison.Ordinal))
						{
							ctx.Fail("invalid token_use");
							return Task.CompletedTask;
						}

						// Valida que o token veio do App Client M2M configurado
						// IMPORTANTE: M2M usa um client_id diferente do client de usuários!
						var clientIdClaim = ctx.Principal?.FindFirst("client_id")?.Value;
						if (!string.Equals(clientIdClaim, m2mClientId, StringComparison.Ordinal))
						{
							ctx.Fail("invalid client_id");
							return Task.CompletedTask;
						}

						return Task.CompletedTask;
					}
				};
			});

		// Define policies de autorização para endpoints M2M
		applicationBuilder.Services.AddAuthorizationBuilder()
			// Policy genérica: aceita qualquer token M2M válido
			// Uso: [Authorize(Policy = "M2M")]
			.AddPolicy("M2M", policy =>
			{
				policy.AuthenticationSchemes.Add(M2MScheme);
				policy.RequireAuthenticatedUser();
			})
			// Policy ESPECÍFICA: requer scope de escrita de telemetria
			// Uso: [Authorize(Policy = "TelemetryWrite")]
			// O scope é configurado no Cognito Resource Server
			.AddPolicy("TelemetryWrite", policy =>
			{
				policy.AuthenticationSchemes.Add(M2MScheme);
				policy.RequireAssertion(context =>
				{
					// Scopes vêm separados por espaço no token: "api/telemetry.write api/read"
					var scopes = context.User.FindFirst("scope")?.Value ?? "";
					return scopes.Contains("api/telemetry.write", StringComparison.Ordinal);
				});
			}) ;
	}
}
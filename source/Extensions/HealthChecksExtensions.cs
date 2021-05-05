using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Mime;
using System.Text.Json;

namespace Health_Checks_API.Extensions
{
	public static class HealthChecksExtensions
	{
		internal const string UIPath = "/monitor";
		internal const string DataUIPath = "/healthchecks-data-ui";
		internal const string TextCheckRouteAPI = "/status-text";
		internal const string JsonCheckRouteAPI = "/status-json";


		public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
		{
			if (configuration is null)
				throw new ArgumentNullException(nameof(configuration));

			services.AddHealthChecks();

			services.AddHealthChecksUI()
					.AddInMemoryStorage();

			return services;
		}

		internal static HealthCheckOptions Options() => 
		new ()
		{
			ResponseWriter = async (context, report) =>
			{
				var result = JsonSerializer.Serialize(
					new
					{
						time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						status = report.Status.ToString(),
					});

				context.Response.ContentType = MediaTypeNames.Application.Json;
				await context.Response.WriteAsync(result);
			}
		};

		internal static HealthCheckOptions UIOptions() =>
		new ()
		{
			Predicate = _ => true,
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		};
	}
}

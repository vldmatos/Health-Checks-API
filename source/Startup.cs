using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Mime;
using System.Text.Json;

namespace Health_Checks_API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "Health_Checks_API", Version = "v1" });
			});

			services.AddHealthChecks();

			services.AddHealthChecksUI()
					.AddInMemoryStorage();
		}

		public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
		{
			if (environment.IsDevelopment())
			{
				application.UseDeveloperExceptionPage();
				application.UseSwagger();
				application.UseSwaggerUI(swagger => swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Health-Checks-API v1"));
			}

			application.UseHealthChecks("/status-text");
			application.UseHealthChecks("/status-json",
						new HealthCheckOptions()
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
						});

			application.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
			{
				Predicate = _ => true,
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
			});
			
			application.UseHealthChecksUI(options =>
			{
				options.UIPath = "/monitor";
			});

			application.UseHttpsRedirection();
			application.UseRouting();
			application.UseAuthorization();
			application.UseEndpoints(endpoints => { endpoints.MapControllers();});
		}
	}
}

using Health_Checks_API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

			services.AddSwagger();

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
				application.UseSwaggerUI(swagger => swagger.SwaggerEndpoint(SwaggerExtension.Url, $"{SwaggerExtension.ApplicationName} {SwaggerExtension.Version}"));
			}

			application.UseHealthChecks(HealthChecksExtensions.TextCheckRouteAPI);
			application.UseHealthChecks(HealthChecksExtensions.JsonCheckRouteAPI, HealthChecksExtensions.Options());
			application.UseHealthChecks(HealthChecksExtensions.DataUIPath, HealthChecksExtensions.UIOptions());			
			application.UseHealthChecksUI(options => { options.UIPath = HealthChecksExtensions.UIPath; });

			application.UseHttpsRedirection();
			application.UseRouting();
			application.UseAuthorization();
			application.UseEndpoints(endpoints => { endpoints.MapControllers();});
		}
	}
}

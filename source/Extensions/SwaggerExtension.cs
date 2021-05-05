using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Health_Checks_API.Extensions
{
	public static class SwaggerExtension
	{
		public const string ApplicationName = "Health_Checks_API";
		public const string Version = "v1";
		public const string Url = "/swagger/v1/swagger.json";

		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc(Version, new OpenApiInfo { Title = ApplicationName, Version = Version });
			});
		}
	}
}

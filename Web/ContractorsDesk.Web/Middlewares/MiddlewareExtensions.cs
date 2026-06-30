using Autofac.Extensions.DependencyInjection;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.FileProviders;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class MiddlewareExtensions
    {
        public static void ConfigureStaticFiles(this WebApplication app)
        {
            var customScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
            var componentsScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Components");

			app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(customScriptsPath),
                RequestPath = "/Views"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(componentsScriptsPath),
                RequestPath = "/Components"
            });
			app.UseStaticFiles();
		}
		public static void ConfigureAutoMapper(this WebApplicationBuilder builder)
		{
			// Register AutoMapper with Microsoft DI
			builder.Services.AddAutoMapper(typeof(Program).Assembly);
			// Use Autofac as the DI container
			builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

			builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
			{
				// Register AutoMapper in Autofac
				containerBuilder.Register(ctx =>
				{
					var config = new MapperConfiguration(cfg =>
					{
						// Scan all assemblies for AutoMapper profiles
						cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
					});

					return config.CreateMapper();
				}).As<IMapper>().InstancePerDependency();
			});
        }
        public static void ConfigureCORS(this WebApplicationBuilder builder)
        {
      //      // Add CORS service and configure it
      //      builder.Services.AddCors(options =>
      //      {
      //          options.AddPolicy("AllowAllOrigins",
      //              policy =>
      //              {
						//policy.AllowAnyOrigin()
						//   .AllowAnyHeader()
						//   .AllowAnyMethod();
      //              });
      //      });

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", policy =>
				{
					policy.WithOrigins("https://localhost:3000")
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials();
				});
			});

		}
	}
}

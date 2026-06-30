using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.WebPortal.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class DbContextMiddleware
	{
		public static void ConfigureDbContext(this WebApplicationBuilder builder)
		{
			builder.Services.AddDbContext<MasterDbContext>((serviceProvider, options) =>
			{
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();

				options.UseSqlServer(configuration.GetConnectionString("master"),
					sqlOptions => sqlOptions.CommandTimeout(3000));

			});

			builder.Services.AddDbContext<ClientDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

				var tenantResolver = serviceProvider.GetRequiredService<TenantResolver>();

				options.UseSqlServer(tenantResolver.GetConnectionString(), sqlOptions => sqlOptions.CommandTimeout(300));

				//options.UseSqlServer(configuration.GetConnectionString("default"),
				//	sqlOptions => sqlOptions.CommandTimeout(3000));

			});
        }
	}
}

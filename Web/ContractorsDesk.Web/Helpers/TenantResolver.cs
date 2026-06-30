using ContractorsDesk.DataStore.Master.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.WebPortal.Helpers
{
	public class TenantResolver
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly MasterDbContext masterDbContext;
		private readonly IConfiguration configuration;
		public TenantResolver(MasterDbContext masterDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.configuration = configuration;
			this.masterDbContext = masterDbContext;
		}
		public string GetConnectionString()
		{
			var httpContext = this.httpContextAccessor.HttpContext;
			var host = httpContext?.Request.Host.Host;

			if (httpContext == null) return string.Empty;

			// Extract the subdomain
			var subdomain = host?.Split('.')[0];

			if (subdomain == "localhost" || host.IndexOf("ngrok-free") > -1) return configuration.GetConnectionString("default");

			// Map subdomain to connection string name
			var connectionString = masterDbContext.Companies.Include(c => c.ConnectionStrings).FirstOrDefault(c => c.SubDomain == subdomain);

			if (connectionString != null && connectionString.ConnectionStrings != null && connectionString.ConnectionStrings.Count > 0)
				return connectionString.ConnectionStrings.First().Value;

			throw new Exception("Connection string not found for subdomain: " + subdomain);
		}

		public string GetSubDomain()
		{
			var httpContext = this.httpContextAccessor.HttpContext;
			var host = httpContext?.Request.Host.Host;

			// Extract the subdomain
			return host?.Split('.')[0];
		}
	}
}

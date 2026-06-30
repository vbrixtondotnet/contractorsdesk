
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class RedisMiddleware
    {
        public static void ConfigureRedis(this WebApplicationBuilder builder)
		{
			//builder.Services.AddStackExchangeRedisCache(options =>
			//{
			//	options.Configuration = builder.Configuration["Redis:ConnectionString"];
			//	options.InstanceName = "SampleInstance:"; // Optional prefix for keys
			//});

			builder.Services.AddTransient<ICacheService>(provider =>
			{
				var configuration = provider.GetRequiredService<IConfiguration>();
				//var idistributedCache = provider.GetRequiredService<IDistributedCache>();
				//return new CacheService(idistributedCache, configuration);
				return new CacheService(configuration);
			});

        }
    }
}

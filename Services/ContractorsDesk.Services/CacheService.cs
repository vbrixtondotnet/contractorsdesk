using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services
{
	public class CacheService : BaseService, ICacheService
	{
		//private readonly IDistributedCache cache;
		//private readonly IConnectionMultiplexer redis;
		private readonly bool IsCacheEnabled;

		// Configure Newtonsoft.Json settings to handle cycles.
		// Option 1: Preserve references (will include $id/$ref in JSON)
		private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
		{
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			// Adjust MaxDepth as needed. (Setting to 1 may be too shallow for many models.)
			MaxDepth = 128
		};
		//public CacheService(IDistributedCache cache, IConfiguration configuration)
		//{
		//	this.cache = cache;
		//	this.IsCacheEnabled = configuration.GetValue<bool>("Redis:Enabled");
		//}
		public CacheService(IConfiguration configuration)
		{
			this.IsCacheEnabled = configuration.GetValue<bool>("Redis:Enabled");
		}
		public async Task<T?> GetFromCache<T>(string key)
		{
			if (this.IsCacheEnabled)
			{
				//var cachedData = await this.cache.GetStringAsync(key);
				//if (cachedData != null)
				//{
				//	return JsonConvert.DeserializeObject<T>(cachedData, _serializerSettings);
				//}
			}

			return default;
		}
		public async Task SetAsync<T>(string key, T value)
		{
			if (this.IsCacheEnabled)
			{
				var serializedData = JsonConvert.SerializeObject(value, _serializerSettings);
				//await this.cache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions());
			}
		}
		public async Task RemoveAsync(string key)
		{
			if (this.IsCacheEnabled)
			{
				//await this.cache.RemoveAsync(key);
			}
		}
	}
}

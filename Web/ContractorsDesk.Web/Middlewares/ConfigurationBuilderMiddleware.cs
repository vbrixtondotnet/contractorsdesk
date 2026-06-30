using ContractorsDesk.Core.Models;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class ConfigurationBuilderMiddleware
	{
		public static void BuildConfigurations(this WebApplicationBuilder builder)
		{
			builder.Services.Configure<TelegramSettingsModel>(builder.Configuration.GetSection("TelegramService"));
		}
	}
}

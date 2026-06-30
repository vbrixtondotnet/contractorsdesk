using Azure;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services.Interfaces;
using Newtonsoft.Json;
using SignNow.Net.Model.Responses.GenericResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services
{
	public class TelegramService : ITelegramService
	{
		private readonly string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
		private readonly TelegramSettingsModel settings;
		public TelegramService(TelegramSettingsModel settings)
		{
			this.settings = settings;
		}
		public async Task<TelegramApiResponseModel> SendMessageAsync(string message)
		{
			message += $"\n\n🧑‍💼 Client Name: `{settings.ClientName}`";
			message += $"\n🏡 Environment: `{environmentName}`";
			message += $"\n🏡 Database: `{settings.DatabaseName}`";
			message += $"\n⏱ Time: `{TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().ToString("yyyy-MM-dd HH:mm:ss")} - California Time`";

			var payload = new
			{
				chat_id = settings.ChatId,
				text = message,
				parse_mode = "Markdown"
			};

			using var client = new HttpClient();
			var json = JsonConvert.SerializeObject(payload);
			var response = await client.PostAsync($"{settings.ApiUrl}{settings.BotToken}/sendMessage",
				new StringContent(json, Encoding.UTF8, "application/json"));

			return await ParseResponse(response);
		}
		public async Task<TelegramApiResponseModel> UpdateMessageAsync(int messageId, string updatedMessage)
		{
			updatedMessage += $"\n\n🧑‍💼 Client Name: `{settings.ClientName}`";
			updatedMessage += $"\n🏡 Environment: `{environmentName}`";
			updatedMessage += $"\n🏡 Database: `{settings.DatabaseName}`";
			updatedMessage += $"\n⏱ Time: `{TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().ToString("yyyy-MM-dd HH:mm:ss")} - California Time`";

			var payload = new
			{
				chat_id = settings.ChatId,
				message_id = messageId,
				text = updatedMessage,
				parse_mode = "Markdown"
			};

			using var client = new HttpClient();
			var json = JsonConvert.SerializeObject(payload);
			var response = await client.PostAsync($"{settings.ApiUrl}{settings.BotToken}/editMessageText",
				new StringContent(json, Encoding.UTF8, "application/json"));

			return await ParseResponse(response);
		}

		private async Task<TelegramApiResponseModel> ParseResponse(HttpResponseMessage response)
		{
			var responseContent = await response.Content.ReadAsStringAsync();
			var telegramResponse = JsonConvert.DeserializeObject<TelegramApiResponseModel>(responseContent);

			return telegramResponse;
		}
	}
}

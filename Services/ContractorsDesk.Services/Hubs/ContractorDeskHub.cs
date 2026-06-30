using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;

namespace ContractorsDesk.Services.Hubs
{
	public class ContractorDeskHub : Hub
	{
		private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();
		private readonly INotificationService notificationService;

		public ContractorDeskHub(INotificationService _notificationService)
        {
            notificationService = _notificationService;
        }

        public async Task PageLoaded()
		{
			await notificationService.CheckForNewNotifications();
		}

		public override async Task OnConnectedAsync()
		{
			var user = Context.User;
			var applicationUser = JsonConvert.DeserializeObject<JObject>(user.Claims.FirstOrDefault(c => c.Type == "ApplicationUser")?.Value);
			string userId = applicationUser.Value<string>("Id");

			if (!string.IsNullOrEmpty(userId))
			{
				_userConnections[userId] = Context.ConnectionId;
			}
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(System.Exception exception)
		{
			var user = Context.User;
			var applicationUser = JsonConvert.DeserializeObject<JObject>(user.Claims.FirstOrDefault(c => c.Type == "ApplicationUser")?.Value);
			string userId = applicationUser.Value<string>("Id");

			if (!string.IsNullOrEmpty(userId))
			{
				_userConnections.Remove(userId);
			}
			await base.OnDisconnectedAsync(exception);
		}

		public async Task SendNotificationToAll(string title, string message)
		{
			await Clients.All.SendAsync(ClientNotificationType.DefaultGlobalNotification.GetStringValue(), title, message);
		}

		public async Task SendNotificationToUser(string userId, string title, string message)
		{
			if (_userConnections.TryGetValue(userId, out string connectionId))
			{
				await Clients.Client(connectionId).SendAsync(ClientNotificationType.DefaultUserNotification.GetStringValue(), title, message);
			}
		}
	}
}

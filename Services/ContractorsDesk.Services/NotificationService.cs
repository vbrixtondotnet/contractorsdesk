using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Hubs;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ContractorsDesk.Core.Utilities;
using System.Collections.Generic;
using ContractorsDesk.Core.Models;
using DocuSign.eSign.Model;
using SignNow.Net.Model;

namespace ContractorsDesk.Services
{
	public class NotificationService : BaseService, INotificationService
	{
		private readonly IHubContext<ContractorDeskHub> hubContext;
		private readonly IHttpContextAccessor httpContext;
		private readonly ITelegramService telegramService;

		public NotificationService(IHubContext<ContractorDeskHub> _hubContext,
			IMapper mapper,
			ClientDbContext clientDataDbContext,
			IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor,
			ITelegramService telegramService)
			: base(mapper: mapper, clientDataDbContext: clientDataDbContext, configuration: configuration)
		{
			hubContext = _hubContext;
			httpContext = httpContextAccessor;
			this.telegramService = telegramService;
		}

		#region Public
		public async Task AddNewNotification(List<UserNotificationPayload> notificationPayloads, NotificationNextActionEnum? nextActionEnum = null)
		{
			foreach(var notificationPayload in notificationPayloads)
			{
				var notification = new UserNotification
				{
					Id = Guid.NewGuid(),
					Title = notificationPayload.Title,
					Message = notificationPayload.Message,
					Description = notificationPayload.Description,
					UserId = notificationPayload.UserId, // Null for global notifications
					IsGlobal = notificationPayload.UserId.HasValue ? false : true,
					CreatedById = notificationPayload.CreatedById,
					RelatedUrl = notificationPayload.RelatedUrl,
					EmailId = notificationPayload.EmailId,
					NextActionEnum = nextActionEnum?.GetStringValue(),
					IsRead = false,
					DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()
				};

				ClientDbContext.UserNotifications.Add(notification);
			}
			
			await ClientDbContext.SaveChangesAsync();

			// Check new notifications
			await CheckForNewNotifications();
		}

		public async Task CheckForNewNotifications()
		{
			var notifications = await ClientDbContext.UserNotifications
				.Where(n => n.IsRead == null || n.IsRead.Value == false)
				.OrderBy(n => n.DateCreated)
				.ToListAsync();

			foreach (var notification in notifications)
			{
				// Send out Client notification
				await SendOutClientNotification(notification);
			}
		}

		public async Task MarkNotificationAsRead(Guid id)
		{
			var notification = await ClientDbContext.UserNotifications
				.Where(n => n.Id == id)
				.FirstOrDefaultAsync();

			if (notification == null) return;

			notification.IsRead = true;
			ClientDbContext.UserNotifications.Update(notification);
			await ClientDbContext.SaveChangesAsync();
		}

		public async Task MarkEmailNotificationAsRead(Guid emailId)
		{
			var notification = await ClientDbContext.UserNotifications
				.Where(n => n.EmailId == emailId)
				.FirstOrDefaultAsync();

			if (notification == null) return;

			notification.IsRead = true;
			ClientDbContext.UserNotifications.Update(notification);
			await ClientDbContext.SaveChangesAsync();
		}

		public async Task<List<UserNotificationPayload>> GenerateUserNotificationPayloads(string prefix, string message, string description, int createdById, string relatedUrl = "", List<int>? userIds = null, List<int>? roleIds = null, Guid? emailId = null)
		{
			var notifications = new List<UserNotificationPayload>();
			var notification = new UserNotificationPayload();

			var isGlobal = roleIds == null && userIds == null ? true : false;

			if (isGlobal)
			{
				notification = CreateNotification(prefix, message, description, createdById, null, relatedUrl, emailId : emailId);
				notifications.Add(notification);
			}
			else
			{
				if (roleIds != null && roleIds.Count > 0)
				{
					var usersByRole = await ClientDbContext.Users
						.Where(ur => roleIds.Contains(ur.RoleId))
						.ToListAsync();

					notifications.AddRange(usersByRole.Select(userRole => CreateNotification(prefix, message, description, createdById, userRole.Id, relatedUrl, emailId: emailId)));
				}

				if (userIds != null && userIds.Count > 0)
				{
					notifications.AddRange(userIds.Select(userId => CreateNotification(prefix, message, description, createdById, userId, relatedUrl, emailId:emailId)));
				}
			}

			return notifications;
		}

		public async Task<List<UserNotificationDto>> GetUserNotifications(int id)
		{
			var notifications = await ClientDbContext.UserNotifications
				.Where(n => n.UserId == id)
				.OrderByDescending(n => n.DateCreated)
				.Take(50)
				.ToListAsync();

			var notificationDtos = notifications.Select(n => new UserNotificationDto
			{
				Id = n.Id,
				Prefix = n.Title,
				Message = n.Message,
				Description = n.Description,
				IsRead = n.IsRead,
				DateCreated = n.DateCreated.ToString(),
				RelatedUrl = n.RelatedUrl,
				EmailId = n.EmailId

			}).ToList();

			return notificationDtos;
		}
		public async Task<int?> SendTelegramMessage(TelegramMessageModel telegramMessage)
		{
			var telegramApiResponse = new TelegramApiResponseModel();
			if (telegramMessage.MessageId == null)
			{
				telegramApiResponse = await telegramService.SendMessageAsync(telegramMessage.Text);
			}
			else
			{
				telegramApiResponse = await telegramService.UpdateMessageAsync(telegramMessage.MessageId.Value, telegramMessage.Text);
			}

			return telegramApiResponse.Result?.MessageId;
		}
		public async Task SendSignalRMessage(SignalRMessageModel signalRMessageModel)
		{
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("*", string.Empty) ?? string.Empty;
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("📁", string.Empty) ?? string.Empty;
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("📒", string.Empty) ?? string.Empty;
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("📄", string.Empty) ?? string.Empty;
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("🚦", string.Empty) ?? string.Empty;
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("🔄", "<span class=\"mini-loader\"></span>");
			signalRMessageModel.Message = signalRMessageModel.Message.Replace("🏡", "");

			if (signalRMessageModel.NotificationId != null)
			{
				var notification = await ClientDbContext.UserNotifications.FirstOrDefaultAsync(n => n.Id == signalRMessageModel.NotificationId);
				notification.Description = signalRMessageModel.Message;

				await ClientDbContext.SaveChangesAsync();
			}
			_ = hubContext.Clients.User(signalRMessageModel.UserId.ToString()).SendAsync("DataSyncNotification", signalRMessageModel);
		}

		public async Task<Guid> SendDataSyncStartedNotification(SignalRMessageModel signalRMessageModel)
		{
			var userNotification = new UserNotification
			{
				Id = Guid.NewGuid(),
				CreatedById = 1,
				UserId = signalRMessageModel.UserId,
				Title = "Data Sync Started",
				Message = "Data Sync Started",
				Description = signalRMessageModel.Message ?? string.Empty,
				IsRead = false,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()
			};
			ClientDbContext.UserNotifications.Add(userNotification);
			await ClientDbContext.SaveChangesAsync();

			_ = hubContext.Clients.User(signalRMessageModel.UserId.ToString()).SendAsync("DataSyncStarted", userNotification);

			return userNotification.Id;
		}

		#endregion

		#region Private
		private async Task SendOutClientNotification(UserNotification notification)
		{
			if (notification == null)
				throw new ArgumentNullException(nameof(notification));

			var createdByUser = !notification.CreatedById.HasValue ? null : await ClientDbContext.Users
				.Where(u => u.Id == notification.CreatedById.Value)
				.SingleOrDefaultAsync();

			var createdBy = createdByUser == null ? string.Empty : $"{createdByUser.FirstName} {createdByUser.LastName}";
			var createdByInitials = createdByUser == null ? string.Empty : $"{createdByUser.FirstName.Substring(0, 1)}{createdByUser.LastName.Substring(0, 1)}";

			var userNotificationDto = new UserNotificationDto 
			{
				Id = notification.Id,
				Prefix = notification.Title,
				Message = notification.Message,
				Description = notification.Description,
				CreatedBy = createdBy,
				DateCreated = notification.DateCreated.ToString(),
				CreatedByInitials = createdByInitials,
				RelatedUrl = notification.RelatedUrl
			};


			if (!notification.UserId.HasValue && notification.IsGlobal.HasValue && notification.IsGlobal.Value)
			{
				var notificationName = ClientNotificationType.GlobalNotification.GetStringValue();
				await hubContext.Clients.All.SendAsync(notificationName, userNotificationDto);
			}

			if (notification.UserId.HasValue)
			{
				var notificationName = ClientNotificationType.UserNotification.GetStringValue();
				await hubContext.Clients.User(notification.UserId.Value.ToString()).SendAsync(notificationName, userNotificationDto);

				if (!string.IsNullOrEmpty(notification.NextActionEnum))
				{
					var notificationNextActionEnum = EnumExtensions.GetEnumValueFromString<NotificationNextActionEnum>(notification.NextActionEnum);

					if (notificationNextActionEnum == NotificationNextActionEnum.ReloadActionItemDashboard)
					{
						notificationName = ClientNotificationType.ReloadActionItemDashboard.GetStringValue();
						await hubContext.Clients.User(notification.UserId.Value.ToString()).SendAsync(notificationName);
					}
				}
			}
		}

		private UserNotificationPayload CreateNotification(string prefix, string message, string description, int createdById, int? userId, string relatedUrl, Guid? emailId = null)
		{
			return new UserNotificationPayload
			{
				Title = prefix,
				Message = message,
				Description = description,
				CreatedById = createdById,
				UserId = userId,
				RelatedUrl = relatedUrl,
				EmailId = emailId
			};
		}
		#endregion
	}
}

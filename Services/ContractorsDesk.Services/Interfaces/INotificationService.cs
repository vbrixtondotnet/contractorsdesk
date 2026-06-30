using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface INotificationService : IBaseService
	{
		Task AddNewNotification(List<UserNotificationPayload> notificationPayloads, NotificationNextActionEnum? nextActionEnum = null);
		Task CheckForNewNotifications();
		Task MarkNotificationAsRead(Guid id);
		Task MarkEmailNotificationAsRead(Guid emailId);
		Task<List<UserNotificationPayload>> GenerateUserNotificationPayloads(string prefix, string message, string description, int createdById, string? relatedUrl = "", List<int>? userIds = null, List<int>? roleIds = null, Guid? emailId = null);
		Task<List<UserNotificationDto>> GetUserNotifications(int id);
		Task<int?> SendTelegramMessage(TelegramMessageModel telegramMessage);
		Task SendSignalRMessage(SignalRMessageModel signalRMessageModel);
		Task<Guid> SendDataSyncStartedNotification(SignalRMessageModel signalRMessageModel);
	}
}

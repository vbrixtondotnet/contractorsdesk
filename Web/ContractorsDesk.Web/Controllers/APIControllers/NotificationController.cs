using Azure;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignNow.Net.Model.Responses.GenericResponses;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[ApiController]
	[Authorize]
	[Route("api")]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	public class NotificationController : ControllerBase
	{
		private readonly ClientDbContext clientDbContext;
		private readonly INotificationService notificationService;

		public NotificationController(ClientDbContext _clientDbContext,
			INotificationService _notificationService)
		{
			clientDbContext = _clientDbContext;
			notificationService = _notificationService;
		}

		[HttpPost("notification-add")]
		public async Task<IActionResult> AddNotification([FromBody] List<UserNotificationPayload> requests)
		{
			//var notificationRequests1 = await notificationService.GenerateUserNotificationPayloads("Test Notification", "Sent by chan test as test for userIds 5 and 58.", 58, "http://localhost:5236/action-items/13", userIds: new List<int> { 5, 58 });
			//var notificationRequests2 = await notificationService.GenerateUserNotificationPayloads("Test Notification", "Sent by chan test as test for all users.", 58, "http://localhost:5236/action-items/13");
			//var notificationRequests3 = await notificationService.GenerateUserNotificationPayloads("Test Notification", "Sent by chan test as test for all users.", 58);

			//await notificationService.AddNewNotification(notificationRequests1);
			//await notificationService.AddNewNotification(notificationRequests2);
			//await notificationService.AddNewNotification(notificationRequests3);

			return Ok(new { Message = "Notification added!" });
		}

		[HttpGet("notification-check")]
		public async Task<IActionResult> CheckNewNotification()
		{
			await notificationService.CheckForNewNotifications();

			return Ok(new { Message = "Notification checked!" });
		}


        [HttpGet("notification-user/{id}")]
        public async Task<IActionResult> GetUserNotifications(int id)
        {
            var response = await notificationService.GetUserNotifications(id);
            return Ok(ApiResponse<List<UserNotificationDto>>.SuccessResponse(response));
        }

        [HttpPut("notification-read/{id}")]
		public async Task<IActionResult> MarkNotificationRead(Guid id)
		{
			await notificationService.MarkNotificationAsRead(id);

			return Ok(ApiResponse<Guid>.SuccessResponse(id));
		}

		[HttpPut("email-notification-read/{emailId}")]
		public async Task<IActionResult> MarkEmailNotificationRead(Guid emailId)
		{
			await notificationService.MarkEmailNotificationAsRead(emailId);

			return Ok(ApiResponse<Guid>.SuccessResponse(emailId));
		}
	}
}

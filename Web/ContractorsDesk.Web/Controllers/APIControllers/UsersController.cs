using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using UserPayload = ContractorsDesk.Core.ApiPayloadModels.UserPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/users")]
	[ApiController]
	public class UsersController : BaseApiController
	{
		private readonly ClientDbContext clientDbContext;
		private INotificationService notificationService;

		public UsersController(
			ClientDbContext clientDbContext,
			INotificationService notificationService,
			IApplicationUserService applicationUserService,
            IHttpContextAccessor httpContextAccessor,
			IAzureStorageService azureStorageService,
            IPermissionsService permissionsService) : 
			base(httpContextAccessor, permissionsService,
				applicationUserService:applicationUserService,
				azureStorageService: azureStorageService)
        {	
			this.clientDbContext = clientDbContext;
			this.notificationService = notificationService;
		}

		[HttpGet]
		public async Task<IActionResult> SearchUserAsync(string? search)
		{
			var result = await applicationUserService.SearchUserAsync(search);
			return Ok(ApiResponse<List<ApplicationUserShortDetailsDto>>.SuccessResponse(result));
		}

		[HttpGet("supervisors")]
		public async Task<IActionResult> GetAllSupervisorsAsync()
		{
			var result = await applicationUserService.GetAllSupervisorsAsync();
			return Ok(ApiResponse<List<ApplicationUserShortDetailsDto>>.SuccessResponse(result));
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> CreateUserAsync([FromBody] UserPayload user)
		{
			if (user.Password != user.ConfirmPassword)
				throw new Exception("Passwords don't match");

			var existingUser = await applicationUserService.GetUserByEmailAddressAsync(user.Email);
			if (existingUser != null)
				throw new Exception($"A user with an email address '{user.Email}' already exists.");

			var userDto = await applicationUserService.CreateAsync<ApplicationUserDto>(user);

			return Ok(ApiResponse<ApplicationUserDto>.SuccessResponse(userDto));
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPut]
		public async Task<IActionResult> UpdateUserAsync([FromBody] UserPayload user)
		{
			var existingUser = await applicationUserService.GetUserByEmailAddressAsync(user.Email);
			if (existingUser != null && existingUser.Id != user.Id)
			{
				throw new Exception($"A user with an email address '{user.Email}' already exists.");
			}

			var userDto = await applicationUserService.UpdateAsync<ApplicationUserDto>(user);
			return Ok(ApiResponse<ApplicationUserDto>.SuccessResponse(userDto));
		}


		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPut("account")]
		public async Task<IActionResult> UpdateUserAccountAsync([FromBody] UserAccountPayload payload)
		{
			payload.Id = this.CurrentUser.Id;

			if(payload.AvatarBase64 != null)
			{
				var avatarUrl = await azureStorageService.UploadFileFromBase64(payload.AvatarBase64, "users", payload.AvatarFileName, $"{payload.Id}/avatars");
				payload.AvatarUrl = avatarUrl;
			}
			else if (payload.RemoveAvatar)
			{
				await azureStorageService.DeleteFileAsync(payload.AvatarUrl);
			}

			var userDto = await applicationUserService.UpdateAccountAsync(payload);
			return Ok(ApiResponse<ApplicationUserDto>.SuccessResponse(userDto));
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPatch("account/change-email")]
		public async Task<IActionResult> ChangeEmailAddress([FromBody] UserAccountChangeEmailPayload payload)
		{
			payload.Id = this.CurrentUser.Id;

			var userDto = await applicationUserService.ChangeEmailAddressAsync(payload);
			return Ok(ApiResponse<ApplicationUserDto>.SuccessResponse(userDto));
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPatch("account/change-password")]
		public async Task<IActionResult> UpdateUserAccountLoginAsync([FromBody] UserAccountChangePasswordPayload payload)
		{
			payload.Id = this.CurrentUser.Id;
			
			var userDto = await applicationUserService.ChangePasswordAsync(payload);
			return Ok(ApiResponse<ApplicationUserDto>.SuccessResponse(userDto));
		}


		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteUserAsync([FromBody] UserPayload user)
		{
			await applicationUserService.DeleteAsync(user);
			return Ok(ApiResponse<string>.SuccessResponse());
		}

		[HttpGet("getUsersByManageJob")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GetUsersByManageAllJobsPermission()
		{
			var result = await applicationUserService.GetUsersByManageAllJobsPermission();
			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(result));
		}

		[HttpPost("notify")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> TestNotifyUser([FromBody] string message)
		{
			var userIds = await clientDbContext.Users.Where(u => u.RoleId == (int)Roles.CompanyOwner).Select(u => u.Id).ToListAsync();
			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = "/"; //await GetActionItemUrl(actionItemDto.Id);

			string notificationMessage = $"<strong>System Administrator:</strong> {message}";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, string.Empty, 1, relatedUrl: relatedUrl, userIds: userIds);

			await notificationService.AddNewNotification(notificationRequest);

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}
	}
}

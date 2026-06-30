using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class UserBookmarksController : ControllerBase
	{
		private readonly IUserBookmarkService userBookmarkService;
		private readonly IMapper mapper;
		public UserBookmarksController(IUserBookmarkService userBookmarkService, IMapper mapper)
		{
			this.userBookmarkService = userBookmarkService;
			this.mapper = mapper;	
		}

		[HttpGet("user-bookmarks")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetUserBookmarks()
		{
			var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

			var response = await userBookmarkService.GetUserBookmarksAsync(userId);
			return Ok(ApiResponse<List<UserBookmarkDto>>.SuccessResponse(response));
		}

		[HttpPost("user-bookmarks")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> CreateUserBookmarkAsync([FromBody] Core.ApiPayloadModels.UserBookmark userBookmark)
		{
			var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			var userBookmarkDto = mapper.Map<UserBookmarkDto>(userBookmark);
			userBookmarkDto.UserId = userId;
			var userBookmarkResult = await userBookmarkService.CreateUserBookmarkAsync(userBookmarkDto); 
			return Ok(ApiResponse<UserBookmarkDto>.SuccessResponse(userBookmarkResult));
		}

		[HttpDelete("user-bookmarks/{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var userBookmarkResult = await userBookmarkService.RemoveBookmark(id);
			return Ok(ApiResponse<bool>.SuccessResponse(userBookmarkResult));
		}


	}
}

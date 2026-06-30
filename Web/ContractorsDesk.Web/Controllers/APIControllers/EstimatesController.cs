using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class EstimatesController : BaseApiController
	{
		private readonly IEstimateService estimateService;
		private readonly IProposalService proposalService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IPostMarkEmailService emailService;
		private readonly IPDFService pDFService;
		private readonly IActionItemsService actionItemsService;
		private readonly INotificationService notificationService;
		public EstimatesController(
			IEstimateService estimateService,
			IProposalService proposalService,
			IClientDocumentService clientDocumentService,
			IAzureStorageService azureStorageService,
			IPostMarkEmailService emailService,
			IPDFService pDFService,
			IActionItemsService actionItemsService,
			INotificationService notificationService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService:azureStorageService)
		{
			this.estimateService = estimateService;
			this.proposalService = proposalService;
			this.clientDocumentService = clientDocumentService;
			this.azureStorageService = azureStorageService;
			this.emailService = emailService;
			this.pDFService = pDFService;
			this.actionItemsService = actionItemsService;
			this.notificationService = notificationService;

			this.estimateService.UserId = this.UserId;
		}

		#region Get
		[HttpGet("estimate-items")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetRevisedEstimates(string name)
		{
			var response = await estimateService.GetEstimateItemsByNameAsync(name);
			return Ok(ApiResponse<List<EstimateCategoryDto>>.SuccessResponse(response));
		}

		[HttpGet("estimate-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetRevisedEstimatesGroup(string name)
		{
			var response = await estimateService.GetEstimateCategoriesByNameAsync(name);
			return Ok(ApiResponse<List<EstimateCategoryDto>>.SuccessResponse(response));
		}

		[HttpGet("estimate-categories/all")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstumateCategories()
		{
			var response = await estimateService.GetAllEstimateCategoriesAsync();
			return Ok(ApiResponse<List<EstimateCategoryDto>>.SuccessResponse(response));
		}

		[HttpGet("estimate-sub-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstimateSubCategories()
		{
			var response = await estimateService.GetAllEstimateCategoriesAsync(false);
			return Ok(ApiResponse<List<EstimateCategoryDto>>.SuccessResponse(response));
		}

		[HttpGet("revised-estimates/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetRevisedEstimates(Guid id)
		{
			var response = await estimateService.GetEstimateToActualAsync(id);
			return Ok(ApiResponse<EstimateToActualDto>.SuccessResponse(response));
		}

		[HttpGet("estimate-categories/project/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstimateCategoriesByProjectId(Guid id)
		{
			var response = await estimateService.GetEstimateCategoriesByProjectId(id);
			return Ok(ApiResponse<List<EstimateCategoryShortDetailsDto>>.SuccessResponse(response));
		}
		#endregion

		#region POST

		[HttpPost("revised-estimates/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveRevisedEstimates(Guid id, [FromBody] List<RevisedEstimateCategoryDto> categories)
		{
			var response = await estimateService.SaveRevisedEstimateAsync(id, categories);
			return Ok(ApiResponse<CostRevisionDto?>.SuccessResponse(response));
		}

		[HttpPost("revised-estimates/mapping")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveRevisedEstimatesMapping([FromBody] RevisedEstimateMappingPayload payload)
		{
			var response = await estimateService.SaveRevisedEstimateMappingAsync(payload);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		
		[HttpPost("estimate-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveEstimateCategory([FromBody] EstimateCategoryPayload payload)
		{
			var response = await estimateService.SaveEstimateCategoryAsync(payload);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		[HttpPatch("revised-estimates/minimum-requested-amount")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateMinimumRequestedAmount([FromBody] MinimumRequestedAmountPayload payload)
		{
			var response = await estimateService.UpdateMinimumRequestedAmount(payload.ProjectId, payload.Amount);
			return Ok(ApiResponse<int>.SuccessResponse(response));
		}


		#endregion

		#region Private
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<ActionItemSummaryViewDto> CreateActionItem(ActionItemPayload actionItem)
			=> await actionItemsService.CreateAsync<ActionItemSummaryViewDto>(actionItem);

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendNewActionItemNotification(ActionItemSummaryViewDto actionItemDto)
		{
			var userIds = actionItemDto.AssignedSupervisors?.Select(s => s.Id).ToList();
			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = await GetActionItemUrl(actionItemDto.Id);
			var notificationMessage = string.Empty;

			var actionItemCreatedBy = actionItemDto.CreatedBy;
			if (actionItemCreatedBy != null)
			{
				notificationMessage = $"<strong>{actionItemDto.CreatedBy}</strong> assigned an Action Item to you.";
			}
			else
			{
				notificationMessage = "An <strong>Action Item</strong> has been assigned to you.";
			}

			var description = $"<p>{actionItemDto.Title}<p>";
			description += $"<p>{actionItemDto.Description}<p>";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, description, 1, relatedUrl: relatedUrl, userIds: userIds);
			await notificationService.AddNewNotification(notificationRequest, NotificationNextActionEnum.ReloadActionItemDashboard);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<string> GetActionItemUrl(int actionItemId)
		{
			return await Task.Run(() => {
				var request = HttpContext.Request;
				var rootUrl = $"{request.Scheme}://{request.Host}";
				var actionItemUrl = $"{rootUrl}/action-items/{actionItemId}";
				return actionItemUrl;
			});
		}
		#endregion


	}
}

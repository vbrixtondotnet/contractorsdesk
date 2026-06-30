using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Hubs;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Mime;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/sysbackgroundjobs")]
    [ApiController]
 
    public class SysBackgroundJobsController : BaseApiController
    {
        private readonly ISysBackgroundJobsService sysBackgroundJobService;
        private readonly IQuickBooksService quickBooksService;
        private readonly IHubContext<ContractorDeskHub> _hubContext;
        private readonly SignalRMessageModel signalRMessageModel;
		private readonly IImportDataService importDataService;
		private readonly IWeeklyNoteActionItemService weeklyNoteActionItemService;

		public SysBackgroundJobsController(
            ISysBackgroundJobsService sysBackgroundJobService, 
            IQuickBooksService quickBooksService,
            IImportDataService importDataService,
			IHubContext<ContractorDeskHub> _hubContext,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor,
			IWeeklyNoteActionItemService weeklyNoteActionItemService) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
        {
            this.sysBackgroundJobService = sysBackgroundJobService;
            this.quickBooksService = quickBooksService;
            this.importDataService = importDataService;
            this._hubContext = _hubContext;
            this.signalRMessageModel = new SignalRMessageModel
			{
				UserId = this.UserId
			};
			this.weeklyNoteActionItemService = weeklyNoteActionItemService;
		}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSysBackgroundJobsAsync()
        {
            var response = await sysBackgroundJobService.GetAllSysBackgroundJobsAsync();
            return Ok(ApiResponse<List<SysBackgroundJobsDto>>.SuccessResponse(response.ToList()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSysBackgroundJobByIdAsync(int id)
        {
            var response = await sysBackgroundJobService.GetSysBackgroundJobAsync(id);
            if (response == null)
            {
                return NotFound(ApiResponse<SysBackgroundJobsDto>.ErrorResponse("SysBackgroundJob not found."));
            }
            return Ok(ApiResponse<SysBackgroundJobsDto>.SuccessResponse(response));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSysBackgroundJobAsync([FromBody] SysBackgroundJobsDto sysBackgroundJobDto)
        {
            if (sysBackgroundJobDto == null)
            {
                return BadRequest(ApiResponse<SysBackgroundJobsDto>.ErrorResponse("SysBackgroundJob data is null."));
            }
            await sysBackgroundJobService.CreateSysBackgroundJobAsync(sysBackgroundJobDto);
            return Ok("Saved successfully.");

        }

        [HttpPost("quickbooksdatasync")]
        public IActionResult ExecuteJob([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
            this.signalRMessageModel.ContainerId = payload.ContainerId;
			this.signalRMessageModel.NotifyOnStart = payload.NotifyOnStart;
			BackgroundJob.Enqueue(() => quickBooksService.RunDataSync(this.signalRMessageModel));
			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("sync-project-totals")]
		public async Task<IActionResult> SyncProjectTotals([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => quickBooksService.SyncProjectTotals(this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}


		[HttpPost("sync-proposals")]
		public async Task<IActionResult> SyncProposals([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => quickBooksService.SyncProposals(this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("sync-active-jobs")]
		public async Task<IActionResult> SyncActiveJobs([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => quickBooksService.SyncActiveJobs(this.signalRMessageModel));
			
			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("sync-production-data")]
		public async Task<IActionResult> SyncProductionData([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => importDataService.ImportDataFromProductionDatabase(this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("sync-qbclass")]
		public async Task<IActionResult> SyncQbClasses([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => quickBooksService.SyncQBClassAsync(true, 15, this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("sync-qbcustomers")]
		public async Task<IActionResult> SyncQBClassAsync([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => quickBooksService.SyncQBCustomerAsync(true, 15, this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("weekly-task-action-item")]
		public async Task<IActionResult> RunWeeklyTaskActionItem([FromBody] QuickBooksDataSyncTriggerPayload payload)
		{
			this.signalRMessageModel.ContainerId = payload.ContainerId;
			BackgroundJob.Enqueue(() => weeklyNoteActionItemService.CreateWeeklyNoteActionItem(this.signalRMessageModel));

			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}



	}
}

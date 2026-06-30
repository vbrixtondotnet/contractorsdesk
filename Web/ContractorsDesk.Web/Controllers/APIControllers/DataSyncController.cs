using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Route("api/data-sync")]
	[ApiController]
	public class DataSyncController : ControllerBase
	{
		private readonly IRecurringJobManager recurringJobManager;
		private readonly ITelegramService telegramService;

		public DataSyncController(IRecurringJobManager recurringJobManager, ITelegramService telegramService)
		{
			this.recurringJobManager = recurringJobManager;
			this.telegramService = telegramService;
		}

		[HttpGet("test-notification")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> TestTelegramNotification()
		{
			await this.telegramService.SendMessageAsync("*TEST MESSAGE*");
			return Ok("Sync job triggered manually.");
		}

		[HttpPost("run")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult GetEstimateDataMappings()
		{
			recurringJobManager.Trigger("Data Sync");
			return Ok("Sync job triggered manually.");
		}
	}
}

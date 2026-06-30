using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class AdministratorController : ControllerBase
	{
		private readonly IPermissionsService _permissionsService;
		public AdministratorController(IPermissionsService permissionsService)
		{
			_permissionsService = permissionsService;
		}

		[HttpPost("permissions")]
		public async Task<IActionResult> PostPermission([FromBody] List<string> parameters)
		{
			System.Threading.Thread.Sleep(2000);
			//throw new NotImplementedException();

			return Ok(new List<string>() { "success","error" });
		}
	}
}

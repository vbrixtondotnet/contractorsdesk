using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class AudioController : BaseController
    {
		private readonly IConfiguration configuration;
		public AudioController(IConfiguration configuration, IServiceProvider provider) 
			: base("Dashboard", provider) 
		{
			this.configuration = configuration;
		}

		[HttpGet]
		[Route("audio/record")]
        public IActionResult Record()
		{
			ViewBag.PageName = "Audio Recorder";
			ViewBag.Action = "/";
			var webHookUrl = configuration["AI:WebHookUrl"];
			ViewBag.WebHookUrl = webHookUrl;

			return RenderView();
        }
	}
}

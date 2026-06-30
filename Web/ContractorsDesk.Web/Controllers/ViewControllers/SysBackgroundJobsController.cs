using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class SysBackgroundJobsController : BaseController
    {
        public SysBackgroundJobsController(IServiceProvider provider) : base("SysBackgroundJobs", provider) { }
        // GET: /sysbackgroundjobs
        [HttpGet]
        [Route("sysbackgroundjobs")]
        public IActionResult SysBackgroundJobs()
        {
            ViewBag.Title = "System Background Jobs";
            ViewBag.PageName = "System Background Jobs";
            ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
            return RenderView();
        }
        [HttpGet("sysbackgroundjobs/{jobId}")]
        public IActionResult JobDetails(string jobId)
        {
            ViewBag.PageName = "System Background Jobs";
            ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "System Background Jobs", Url = "/sysbackgroundjobs" };
            ViewBag.JobId = jobId;
            return RenderView();
        }

    }
}

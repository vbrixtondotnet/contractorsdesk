using ContractorsDesk.Core.Dto;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	public abstract class BaseController : Controller
	{
		private string parentRoute;
        public readonly IHttpContextAccessor httpContextAccessor;

        public int UserId { get; set; }
		public int RoleId { get; set; }
		public int? CompanyId { get; set; }
		public int RoleCategoryId { get; set; }
		public Roles? CurrentRole { get; set; }
		public List<string>? UserPermissions { get; set; }
		public ApplicationUserModel CurrentUser { get; set; }
		public CompanySettingDto? CompanySettings { get; set; }
		public PermissionViewModel Permissions { get; set; } = new PermissionViewModel();
        public BaseController() { }
        public BaseController(string parentRoute, IServiceProvider provider)
		{
			this.parentRoute = parentRoute;
            this.httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

			var httpContext = httpContextAccessor.HttpContext;
			var userSession = httpContext?.User;
			var appUserClaim = userSession?.FindFirst("ApplicationUser")?.Value;
			var permissions = userSession?.FindFirst("Permissions")?.Value;

			if (!string.IsNullOrEmpty(appUserClaim))
			{
				var appUser = JsonSerializer.Deserialize<ApplicationUserModel>(appUserClaim);

				if (appUser == null)
					httpContext.Response.Redirect("/signout");

				if (appUser != null)
				{
					this.CurrentUser = appUser;
					this.RoleId = appUser.RoleId;
					this.UserId = Convert.ToInt32(userSession?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
					this.CompanyId = appUser.CompanyId;
					this.RoleCategoryId = appUser.RoleCategoryId;
					this.CurrentRole = GetRole(this.RoleId);

					var userPermissions = httpContext?.Request.Cookies["contractorsdesk_permissions"];
					var companySettings = httpContext?.Request.Cookies["contractorsdesk_companysetting"];

					if (companySettings == null || userPermissions == null)
					{
						httpContext?.Response.Redirect("/signout");
					}
					else
					{
						CompanySettings = JsonSerializer.Deserialize<CompanySettingDto>(companySettings);
						this.UserPermissions = JsonSerializer.Deserialize<List<string>>(userPermissions);
						this.Permissions.UserRole = this.CurrentRole.GetStringValue();
						this.Permissions.CanManageAllJobs = this.HasPermission("Manage All Jobs");
						this.Permissions.CanManageOwnJobs = this.HasPermission("Manage Own Jobs");
						this.Permissions.CanManageAllActionItems = this.HasPermission("Manage All Action Items");
						this.Permissions.CanManageOwnActionItems = this.HasPermission("Manage Own Action Items");
						this.Permissions.CanManageCompanyUsers = this.HasPermission("Manage Company Users");
						this.Permissions.CanManageCompanyRoles = this.HasPermission("Manage Company Roles");
						this.Permissions.CanManageSystemUsers = this.HasPermission("Manage System Users");
						this.Permissions.CanManageSystemRoles = this.HasPermission("Manage System Roles");
						this.Permissions.CanManageOwnEstimates = this.HasPermission("Manage Own Estimates");
						this.Permissions.CanManageAllEstimates = this.HasPermission("Manage All Estimates");
						this.Permissions.CanAssignJobs = this.HasPermission("Can Assign Jobs");
						this.Permissions.CanAssignEstimates = this.HasPermission("Can Assign Estimates");
						this.Permissions.CanAssignActionItems = this.HasPermission("Can Assign Action Items");
						this.Permissions.CanAccessClientJobs = this.HasPermission("Can Access Client Jobs");
						this.Permissions.CanManageDataMapping = this.HasPermission("Can Manage Data Mapping");
						this.Permissions.CanEditAcceptedProposals = this.HasPermission("Can Edit Accepted Proposals");
						this.Permissions.CanEditCompletedSchedules = this.HasPermission("Can Edit Completed Schedules");
						this.Permissions.CanAccessCompanySettings = this.HasPermission("Can Access Company Settings");
						this.Permissions.CanAccessDataSyncServices = this.HasPermission("Can Access Data Sync Services");
					}
					
                }
			}

		}
		[NonAction]
		public ViewResult RenderView()
		{
			ViewBag.CurrentUser = this.CurrentUser;
			ViewBag.Permissions = this.Permissions;
			ViewBag.Role = this.CurrentRole;
			ViewBag.CompanySettings = this.CompanySettings;

            return OnRenderView(null);
		}
		[NonAction]
		public ViewResult RenderView(object? model)
		{
			ViewBag.CurrentUser = this.CurrentUser;
			ViewBag.Permissions = this.Permissions;
			ViewBag.Role = this.CurrentRole;
            ViewBag.CompanySettings = this.CompanySettings;
            return OnRenderView(model);
		}
		private ViewResult OnRenderView(object? model)
		{
			ViewBag.Path = Request.Path;
			var fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

			var routeValues = Request.RouteValues.ToList();

			var controller = routeValues.FirstOrDefault(r => r.Key == "controller").Value;
			var action = routeValues.FirstOrDefault(r => r.Key == "action").Value;

			ViewBag.ControllerName = this.parentRoute ?? controller;
			ViewBag.Action = action.ToString() == "Dashboard" ? "/" : action;
			return View($"~/Views/{controller}/{action}/{action}.view.cshtml", model);
		}
		private Roles? GetRole(int roleId)
		{
			if (Enum.IsDefined(typeof(Roles), roleId))
			{
				return (Roles)roleId;
			}
			else
			{
				return null;
			}
		}
		private bool HasPermission(string permission)
		{
			if (this.UserPermissions == null) return false;

			return this.UserPermissions.Any(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
		}
		protected void InitializeCompanySettings()
		{	
			ViewBag.CompanySettings = this.CompanySettings;
		}
    }
}

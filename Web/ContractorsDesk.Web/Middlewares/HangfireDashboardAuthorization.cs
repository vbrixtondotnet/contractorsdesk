using Hangfire.Dashboard;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using ContractorsDesk.Core.Enums;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class DataSyncDashboardAuthorization : IDashboardAuthorizationFilter
	{
		public bool Authorize(DashboardContext context)
		{
			var httpContext = context.GetHttpContext();

			if (httpContext.User.Identity?.IsAuthenticated ?? false)
			{
				var userClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationUser")?.Value;
				JObject? applicationUser = null;

				if (!string.IsNullOrEmpty(userClaim))
				{
					try { applicationUser = JObject.Parse(userClaim); }
					catch (JsonException) { return false; }
				}

				var role = applicationUser?["Role"]?.ToString() ?? "";
				var userRole = EnumExtensions.GetEnumValueFromString<Roles>(role);
				var allowedRoles = new[]
				{
					Roles.SuperAdmin,
					Roles.SuperIT,
					Roles.CompanyOwner,
					Roles.CompanyIT
				};

				if (allowedRoles.Contains(userRole))
					return true;
			}

			return false;
		}
	}

    public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return false;
        }
    }
}

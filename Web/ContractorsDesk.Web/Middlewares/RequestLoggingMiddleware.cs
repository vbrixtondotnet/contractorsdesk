using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.WebPortal.Controllers.APIControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class RequestLoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestLoggingMiddleware> _logger;

		public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, ClientDbContext db)
		{
			var endpoint = context.GetEndpoint();
			var controllerType = endpoint?.Metadata
				.OfType<ControllerActionDescriptor>()
				.FirstOrDefault()?.ControllerTypeInfo;

			if (!controllerType?.IsSubclassOf(typeof(ContractorsDesk.WebPortal.Controllers.ViewControllers.BaseController)) == true 
				|| context.Request.Path.ToString().Contains("/contractorDeskHub", StringComparison.OrdinalIgnoreCase)
				|| context.Request.Path.ToString().Contains("/assets/", StringComparison.OrdinalIgnoreCase))
			{
				await _next(context); // Skip logging
				return;
			}

			var userSession = context?.User;
			var appUserClaim = userSession?.FindFirst("ApplicationUser")?.Value;
			var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


			if (!string.IsNullOrEmpty(userId))
			{
				var url = context.Request.Path + context.Request.QueryString;
				var userlog = new UserLog();
				userlog.Id = Guid.NewGuid();
				userlog.UserId = Convert.ToInt32(userSession?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
				userlog.Url = url;
				db.UserLogs.Add(userlog);
				await db.SaveChangesAsync();
			}

			await _next(context);
		}
	}


}

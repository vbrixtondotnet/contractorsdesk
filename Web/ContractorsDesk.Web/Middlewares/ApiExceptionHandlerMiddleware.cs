using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class ApiExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next; 
		private readonly IWebHostEnvironment _env;
		private readonly ILogger<ApiExceptionHandlerMiddleware> _logger;


		public ApiExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ApiExceptionHandlerMiddleware> logger)
		{
			_next = next;
			_env = env;
			_logger = logger;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				await HandleExceptionAsync(httpContext, ex);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var endpoint = context.GetEndpoint();
			var controllerType = endpoint?.Metadata
				.OfType<ControllerActionDescriptor>()
				.FirstOrDefault()?.ControllerTypeInfo;

			if (controllerType?.IsSubclassOf(typeof(Controllers.APIControllers.BaseApiController)) == true)
			{
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				// Create a custom API response model (you can customize this based on your needs)
				var message = exception.Message;//_env.IsDevelopment() || exception.Message == "401" ? exception.Message : "An unexpected error occurred. Please try again later.";
				var response = ApiResponse<ApiResponseModel>.ErrorResponse(message);

				var settings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					Formatting = Formatting.Indented  // Optional: for pretty print formatting
				};

				var result = JsonConvert.SerializeObject(response, settings);
				return context.Response.WriteAsync(result);
			}

			return Task.CompletedTask;

		}
	}
}

using System.Diagnostics;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class RequestTimingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestTimingMiddleware> _logger;

		public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			var stopwatch = Stopwatch.StartNew();

			await _next(context); // Proceed to next middleware/controller

			stopwatch.Stop();

			var elapsedMs = stopwatch.ElapsedMilliseconds;
			var path = context.Request.Path;

			_logger.LogInformation("Request to {Path} took {Elapsed} ms", path, elapsedMs);
		}
	}

}

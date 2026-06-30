namespace ContractorsDesk.WebPortal.Middlewares
{
	public class AuthMiddleWare
	{
		private readonly RequestDelegate _next;

		public AuthMiddleWare(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var originalResponseStream = context.Response.Body;

			using (var newResponseStream = new MemoryStream())
			{
				context.Response.Body = newResponseStream;

				await _next(context);

				if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
				{
					context.Response.Redirect("/login");
				}
				else
				{
					// Copy the newResponseStream to the original response stream
					newResponseStream.Seek(0, SeekOrigin.Begin);
					await newResponseStream.CopyToAsync(originalResponseStream);
				}
			}
		}
	}

}

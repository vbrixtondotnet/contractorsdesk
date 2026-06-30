using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class JwtMiddleware
	{
		private readonly RequestDelegate _next;

		public JwtMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
		{
			var originalResponseStream = context.Response.Body;

			using (var newResponseStream = new MemoryStream())
			{
				context.Response.Body = newResponseStream;
				await _next(context);

				if (context.Request.Cookies.TryGetValue("jwt", out var token))
				{
					var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
					var tokenHandler = new JwtSecurityTokenHandler();

					try
					{
						var validationParameters = new TokenValidationParameters
						{
							ValidateIssuer = false,
							ValidateAudience = false,
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(key)
						};

						var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

						if (principal.Identity.IsAuthenticated)
						{
							context.Response.StatusCode = 200;
							//return;
							//context.Response.StatusCode = 200;
							newResponseStream.Seek(0, SeekOrigin.Begin);
							await newResponseStream.CopyToAsync(originalResponseStream);
						}
					}
					catch (Exception)
					{
						context.Response.StatusCode = 401;
						context.Response.Cookies.Delete("jwt"); 
						context.Response.Redirect("/login");
					}
				}
				else
				{
					context.Response.StatusCode = StatusCodes.Status200OK;
					context.Response.Redirect("/login");
				}

			}

			
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ContractorsDesk.WebPortal.Attributes
{
	public class AuthorizeViewAttribute: Attribute, IAuthorizationFilter

	{
		private readonly string _requiredRole;

		public AuthorizeViewAttribute(string requiredRole = "")
		{
			_requiredRole = requiredRole;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

			if (context.HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
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
						return;
					}
					else
					{
						context.Result = new RedirectResult("/login");
					}
				}
				catch (Exception)
				{
					context.Result = new RedirectResult("/login");
				}
			}
			else
			{
				context.Result = new RedirectResult("/login");
			}
		}
	}
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class JWTAuthenticationConfigurationMiddleware
	{
		public static void ConfigureJWTAuthentication(this WebApplicationBuilder builder)
		{
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.UseSecurityTokenValidators = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					ClockSkew = TimeSpan.Zero
				};
				options.SaveToken = true;
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						// Log the error or handle it in a custom way
						Console.WriteLine($"Authentication failed: {context.Exception.Message}");

						// You can return a custom response if needed
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("Token-Expired", "true");
						}

						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						// Suppress the automatic redirect
						context.HandleResponse();

						if (context.HttpContext.Items.TryGetValue("ForceSignOut", out var shouldSignOut) && (bool)shouldSignOut)
						{
							context.Response.Redirect("/signout");
							return Task.CompletedTask;
						}

						if (context.Request.Path.ToString().Contains("api"))
							context.Response.StatusCode = 401; // Send 401 Unauthorized instead of redirect
						else
							context.Response.Redirect("/login");

						return Task.CompletedTask;
					},
					OnMessageReceived = context =>
					{
						// Try to get the token from the Authorization header
						var authorizationHeader = context.Request.Headers["Authorization"].ToString();
						if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
						{
							// Extract token from Authorization header
							context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
						}
						else
						{
							// Fallback to token from the contractorsdesk_access_token cookie if the Authorization header is not present or invalid
							var tokenFromCookie = context.Request.Cookies["contractorsdesk_access_token"];
							if (!string.IsNullOrWhiteSpace(tokenFromCookie))
							{
								context.Token = tokenFromCookie;
							}
						}

						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						var deploymentVersion = context.HttpContext.RequestServices
							.GetRequiredService<IConfiguration>()["DeploymentVersion"];

						var jwtVersion = context.Principal?.FindFirst("DeploymentVersion")?.Value;

						if (jwtVersion != deploymentVersion)
						{
							context.HttpContext.Items["ForceSignOut"] = true;
							context.Fail("Token invalid due to logout version mismatch.");
						}

						return Task.CompletedTask;
					}
				};
			});

		}
	}
}

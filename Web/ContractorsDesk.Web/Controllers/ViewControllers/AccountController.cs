using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractorsDesk.Core.Models;
using Microsoft.AspNetCore.Identity;
using ContractorsDesk.WebPortal.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Utilities;
using System.Text.Json.Serialization;
using PostmarkDotNet.Model;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly IConfiguration configuration;
		private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
		private readonly IApplicationUserService applicationUserService;
		private readonly ICompanySettingService companySettingService;


		public AccountController(
			IApplicationUserService applicationUserService,
			ICompanySettingService companySettingService,
			IConfiguration configuration, 
			IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
		{
			this.configuration = configuration;
			this.companySettingService = companySettingService;
			this.httpContextAccessor = httpContextAccessor;
			this.mapper = mapper;
			this.applicationUserService = applicationUserService;

        }

		#region Public
		
		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("login")]
		public IActionResult Login()
		{	
			if (User.Identity.IsAuthenticated)
			{
				return Redirect("/");
			}

			return View();
		}

		[AllowAnonymous]
		[HttpPost("signin")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await applicationUserService.GetUserByEmailAddressAsync(model.Email);

				if(user != null)
				{
					if (await applicationUserService.CheckPasswordAsync(user, model.Password))
					{
						await this.applicationUserService.SetLogOnRequirementAsync(user.Id, false);

						var claims = await CreateClaims(user);
						var token = await CreateToken(claims);
						var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

						Response.Cookies.Append("contractorsdesk_access_token", tokenString, new CookieOptions
						{
							HttpOnly = false,
							Secure = true,   // Use only with HTTPS
							SameSite = SameSiteMode.None,
							Expires = token.ValidTo
						});

						var rolePermissionString = JsonSerializer.Serialize(user.Role.RolePermissions.Select(p => p.Permission.Description).ToList());
						Response.Cookies.Append("contractorsdesk_permissions", rolePermissionString, new CookieOptions
						{
							HttpOnly = true, // Prevents JavaScript access
							Secure = true,   // Ensures it's sent over HTTPS
							SameSite = SameSiteMode.Strict, // Prevents cross-site leakage
							Expires = token.ValidTo // Optional expiration
						});

						var companySettings = await companySettingService.GetCompanySettingAsync();
						var companySettingsString = JsonSerializer.Serialize(companySettings);
						Response.Cookies.Append("contractorsdesk_companysetting", companySettingsString, new CookieOptions
						{
							HttpOnly = true, // Prevents JavaScript access
							Secure = true,   // Ensures it's sent over HTTPS
							SameSite = SameSiteMode.Strict, // Prevents cross-site leakage
							Expires = token.ValidTo // Optional expiration
						});

						var userDto = mapper.Map<ApplicationUserDto>(user);
						return Ok(new { Token = tokenString, User = userDto });
					}
				}


			}

			return BadRequest("Email Address or Password is incorrect!");
		}

		[AllowAnonymous]
		[HttpGet("signout")]
		public async Task<IActionResult> SignOut()
		{
			HttpContext.Session.Clear();

			HttpContext.Response.Cookies.Delete("contractorsdesk_access_token", new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None
			});

			HttpContext.Response.Cookies.Delete("contractorsdesk_permissions", new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None
			});

			HttpContext.Response.Cookies.Delete("contractorsdesk_companysetting", new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None
			});

			return Redirect("/login");
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("forgot-password")]
		public IActionResult ForgotPassword()
		{
			if (User.Identity.IsAuthenticated)
			{
				// Redirect to Home or another page if already logged in
				return Redirect("/");
			}
			return View();
		}

		[AllowAnonymous]
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
		{
			var user = await this.applicationUserService.GetUserByEmailAddressAsync(model.Email);

			if (user != null)
			{
				var token = Guid.NewGuid(); //await this.userManager.GeneratePasswordResetTokenAsync(user);
				var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

				var userResetPasswordDto = new UserResetPasswordDto
				{
					Email = user.Email,
					ResetLink = resetLink
				};
				
				await this.applicationUserService.UserResetPasswordRequest(userResetPasswordDto);
			}

			return Ok();
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("reset-password")]
		public IActionResult ResetPassword(string token, string email, Guid id)
		{
			if (User.Identity.IsAuthenticated)
				return Redirect("/");

			if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || id == Guid.Empty)
				return Redirect("/");

			return View(new ResetPasswordModel { Token = token, Email = email, Id = id });
		}

		[AllowAnonymous]
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = await this.applicationUserService.GetUserByEmailAddressAsync(model.Email);

			if (user == null)
				return BadRequest(new ResetPasswordModel { ModelError = new Dictionary<string, string> { { "Invalid", "Invalid user email." } } });

			var userResetPasswordDto = new UserResetPasswordDto
			{
				Id = model.Id,
				Email = user.Email,
				Token = model.Token,
				Password = model.Password,
				ConfirmPassword = model.ConfirmPassword
			};

			var resetPassword = await this.applicationUserService.UserResetPassword(user, userResetPasswordDto);

			if (!resetPassword.Item1)
				return BadRequest(new ResetPasswordModel { ModelError = resetPassword.Item2 });

			return Ok();
		}

        [AllowAnonymous]
        [HttpPut("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await this.applicationUserService.GetUserByConfirmationCodeAsync(model.ConfirmationCode);

            if (user == null || user.Status != 0)
            {
                return BadRequest(new ConfirmAccountModel
                {
                    ModelError = new Dictionary<string, string>
            {
                { "Invalid", "Invalid or already confirmed user." }
            }
                });
            }

            await applicationUserService.ConfirmAccount(model.ConfirmationCode, model.Password, user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("confirm-client-account")]
        public IActionResult ClientAccount()
        {
            return View();
        }

        #endregion

        #region Private
        private async Task<List<Claim>> CreateClaims(ApplicationUser user)
		{
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Email ?? "")
			};

            if (user.Role != null)
			{
				try
				{
					var applicationUser = new ApplicationUserModel
					{
						Id = user.Id,
						Email = user.Email,
						FirstName = user.FirstName,
						LastName = user.LastName,
						Role = user.Role.Name,
						RoleCategoryId = user.Role.RoleType,
						RoleId = user.Role.Id,
						AvatarUrl = user.AvatarUrl
					};
					var applicationUserString = JsonSerializer.Serialize(applicationUser);

					claims.Add(new Claim("ApplicationUser", applicationUserString));
					claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
					claims.Add(new Claim("DeploymentVersion", configuration["DeploymentVersion"]));
				}
				catch (Exception ex)
				{

				}
			}

			return claims;
		}
		private async Task<JwtSecurityToken> CreateToken(List<Claim> claims)
		{
			var expirationTime = DateTime.UtcNow.AddDays(30);
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			return new JwtSecurityToken(
				issuer: configuration["Jwt:Issuer"],
				audience: configuration["Jwt:Audience"],
				notBefore: TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				expires: expirationTime,
				claims: claims,
				signingCredentials: creds
			);
		}
		#endregion

	}
}

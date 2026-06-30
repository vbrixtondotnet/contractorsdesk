using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContractorsDesk.Core.Models;
using Microsoft.AspNetCore.Authorization;
using ContractorsDesk.Services.Interfaces;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("api")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly IApplicationUserService applicationUserService;

		public AuthController(IConfiguration configuration, IApplicationUserService applicationUserService)
		{
			this.configuration = configuration;
			this.applicationUserService = applicationUserService;
		}

		[Authorize]
		[HttpGet]
		[Route("createuser")]
		public async Task<IActionResult> Create(string email, string password)
		{
			var user = new ApplicationUser { Email = password, FirstName = email, LastName = email };
			//var result = await _userManager.CreateAsync(user, password);
			return Ok();
		}

        [AllowAnonymous]
        [HttpGet]
        [Route("database-name")]
        public async Task<IActionResult> GetDatabaseName()
        {
			var databaseName = await applicationUserService.GetDatabaseName();
            return Ok(ApiResponse<string>.SuccessResponse(databaseName));
        }
        private string GenerateJwtToken(string username)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var token = new JwtSecurityToken(
				issuer: configuration["Jwt:Issuer"],
				audience: configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

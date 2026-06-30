using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class ApplicationUserService : BaseService, IApplicationUserService
	{
		private readonly IPostMarkEmailService _emailService;
		private readonly INotificationService notificationService;

		public ApplicationUserService(
			IMapper mapper, 
			ClientDbContext clientDataDbContext,
			MasterDbContext masterDbContext,
			IConfiguration configuration,
			IPostMarkEmailService emailService,
			INotificationService _notificationService)
			: base(mapper, clientDataDbContext, masterDbContext:masterDbContext, configuration: configuration)
		{
			this._emailService = emailService;
			this.notificationService = _notificationService;
		}

		public async Task<List<ApplicationUserShortDetailsDto>> SearchUserAsync(string? key)
		{
			var users = await this.ClientDbContext.Users
				.Where(u => (key == null) || (u.FirstName.Contains(key) || u.LastName.Contains(key) || u.Email.Contains(key)) && !u.IsDeleted)
				.Select(u => new ApplicationUserShortDetailsDto
				{
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Email = u.Email
				})
				.ToListAsync();

			return users;
		}
		public async Task<List<ApplicationUserDto>> GetUsersByRoleCategoryAsync(int roleCategoryId)
		{
			var usersInRoleCategory = await this.ClientDbContext.Users
					.Include(u => u.Role)
					.ThenInclude(u => u.RolePermissions)
					.ThenInclude(u => u.Permission)	
					.Where(u => u.Role.RoleType == roleCategoryId && !u.IsDeleted)  
					.ToListAsync();

			return this.mapper.Map<List<ApplicationUserDto>>(usersInRoleCategory);
		}		
		public async Task<List<ApplicationUserDto>> GetUsersByCompanyAsync(int? companyId)
		{
			var companyUsers = await ClientDbContext.Users
				.Include(u => u.Role)
				.ThenInclude(ur => ur.RolePermissions)
                .ThenInclude(ur => ur.Permission)
                .Where(u => u.Role.RoleType != 1 && !u.IsDeleted)
				.ToListAsync();

			return this.mapper.Map<List<ApplicationUserDto>>(companyUsers);
		}
		public async Task<List<ApplicationUserDto>> GetUsersByRoleAsync(int roleId)
		{
			var users = await ClientDbContext.Users.Where(u=> u.RoleId == roleId && !u.IsDeleted).ToListAsync();
			return this.mapper.Map<List<ApplicationUserDto>>(users);
		}
		public async Task<ApplicationUserDto> GetUserByIdAsync(int id)
		{
			var dbUser = await ClientDbContext.Users.FirstOrDefaultAsync(u=> u.Id == id);
			return this.mapper.Map<ApplicationUserDto>(dbUser);
		}
		public async Task<AccountDetailsDto> GetAccountDetailsAsync(int id)
		{
			var dbUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
			return this.mapper.Map<AccountDetailsDto>(dbUser);
		}
		public async Task<ApplicationUser> GetUserByEmailAddressAsync(string emailAddress)
		{
            var dbUser = await ClientDbContext.Users
				.Include(u=> u.Role)
				.ThenInclude(Role => Role.RolePermissions)
				.ThenInclude(Role => Role.Permission)
				.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == emailAddress.Trim().ToLower() && u.IsDeleted != true);
            return this.mapper.Map<ApplicationUser>(dbUser);
        }
        public async Task<List<ApplicationUserDto>> GetUsersByManageAllJobsPermission()
        {
            var dbUsersWithPermission = await ClientDbContext.Users
                .Include(u => u.Role)
                .ThenInclude(ur => ur.RolePermissions)
                .ThenInclude(ur => ur.Permission)
                .Where(u => u.Role.RolePermissions.Any(rp => rp.Permission.Description == "Manage All Jobs") && !u.IsDeleted)
                .ToListAsync();

            var userDtos = dbUsersWithPermission.Select(user => new ApplicationUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedDate = user.DateCreated
            }).ToList();

            return this.mapper.Map<List<ApplicationUserDto>>(userDtos);
        }
        public async Task<ApplicationUser> GetUserByConfirmationCodeAsync(string code)
        {
            var dbUser = await ClientDbContext.Users
                .Include(u => u.Role)
                .ThenInclude(Role => Role.RolePermissions)
                .ThenInclude(Role => Role.Permission)
                .FirstOrDefaultAsync(u => u.ConfirmationCode.Trim().ToLower() == code.Trim().ToLower() && u.IsDeleted != true);
            return this.mapper.Map<ApplicationUser>(dbUser);
        }
        public async Task<bool> SetLogOnRequirementAsync(int id, bool value)
		{
			var dbUser = await this.ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
			if(dbUser != null)
			{
				dbUser.RequireLogin = value;
				await ClientDbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public async Task UserResetPasswordRequest(UserResetPasswordDto passwordRequestDto)
		{
			var resetId = Guid.NewGuid();
			var resetLink = $"{passwordRequestDto.ResetLink}&id={resetId}";
			var emailModel = new EmailPayloadModel
			{
				To = passwordRequestDto.Email,
				Subject = $"{configuration["GmailSmtpServer:AppName"] ?? string.Empty} - Reset password:",
				Body = GenerateResetPasswordEmailBody(resetLink)
			};

			var emailSent = await _emailService.SendEmailAsync(emailModel);

			if (emailSent)
			{
				var dbUserResetPasswordRequest = new UserResetPasswordRequest();
				dbUserResetPasswordRequest.Id = resetId;
				dbUserResetPasswordRequest.Email = passwordRequestDto.Email;
				dbUserResetPasswordRequest.DateSent = DateTime.UtcNow;
				dbUserResetPasswordRequest.IsUsed = false;
				dbUserResetPasswordRequest.SentStatus = emailSent ? "Sent" : "Pending";
				dbUserResetPasswordRequest.ResetLink = resetLink;

				ClientDbContext.UserResetPasswordRequests.Add(dbUserResetPasswordRequest);
				await ClientDbContext.SaveChangesAsync();
			}
		}
		/// <summary>
		/// Reset the user password.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="passwordRequestDto"></param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="Tuple<bool, Dictionary<string, string>>"/>
		/// Item1: <see cref="bool"/>, represents the password reset status
		/// Item2: <see cref="Dictionary<string, string>"/>, holds the error details if occurred.
		/// </returns>
		public async Task<Tuple<bool, Dictionary<string, string>>> UserResetPassword(ApplicationUser user, UserResetPasswordDto passwordRequestDto)
		{
			var appUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
			
			if (appUser == null)
                return Tuple.Create(false, new Dictionary<string, string> { { "Invalid", "Invalid user." } });

			var userResetPasswordRequest = await ClientDbContext.UserResetPasswordRequests
				.Where(i => i.Id == passwordRequestDto.Id)
				.SingleOrDefaultAsync();

			if (userResetPasswordRequest == null)
				return Tuple.Create(false, new Dictionary<string, string> { { "Invalid", "No password reset request." } });

			if (userResetPasswordRequest.IsUsed.HasValue && userResetPasswordRequest.IsUsed.Value)
				return Tuple.Create(false, new Dictionary<string, string> { { "Invalid", "You have already reset the password using this link, please request again." } });

			if (userResetPasswordRequest.DateSent.AddHours(1) < DateTime.UtcNow)
				return Tuple.Create(false, new Dictionary<string, string> { { "Invalid", "Password reset link expired." } });


            userResetPasswordRequest.IsUsed = true;

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            appUser.Password = passwordHasher.HashPassword(user, passwordRequestDto.Password);

            await ClientDbContext.SaveChangesAsync();

            return Tuple.Create(true, new Dictionary<string, string> { { "Valid", "You have successfully reset your password." } });

		}
		public override async Task<T> CreateAsync<T>(object param)
		{
			if (param is Core.ApiPayloadModels.UserPayload userPayload)
			{
				var user = mapper.Map<DataStore.Client.Models.User>(userPayload);
                var passwordHasher = new PasswordHasher<DataStore.Client.Models.User>();
                
                user.Password = passwordHasher.HashPassword(user, userPayload.Password);

				ClientDbContext.Users.Add(user);
				await ClientDbContext.SaveChangesAsync();
                return this.mapper.Map<T>(user);
			}
			else
			{
				throw new ArgumentException("Invalid payload type for CreateAsync");
			}
		}
		public override async Task<T> UpdateAsync<T>(object param)
		{
			if (param is Core.ApiPayloadModels.UserPayload updateUserPayload)
			{
				var applicationUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == updateUserPayload.Id);
				//var applicationUser = await _unitOfWork.Context.Users.FirstOrDefaultAsync(u => u.Id == updateUserPayload.Id);
				if (applicationUser == null) throw new Exception("User not found.");

				mapper.Map(updateUserPayload, applicationUser);

                await ClientDbContext.SaveChangesAsync();			
				return this.mapper.Map<T>(applicationUser);
			}
			else
			{
				throw new ArgumentException("Invalid payload type for CreateAsync");
			}
		}
		public override async Task DeleteAsync(object param)
		{
			if (param is Core.ApiPayloadModels.UserPayload userDeletePayload)
			{
				var applicationUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == userDeletePayload.Id);
				if (applicationUser == null) throw new Exception("User not found.");

				applicationUser.IsDeleted = true;
				await ClientDbContext.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException("Invalid payload type for CreateAsync");
			}
		}
		public async Task<List<ApplicationUserShortDetailsDto>> GetAllSupervisorsAsync()
		{
			List<int> permissionIds = [(int)Permissions.ManageAllJobs, (int)Permissions.ManageOwnJobs];

			return await ClientDbContext.Users
				.Where(u => !u.IsDeleted && ClientDbContext.RolePermissions
					.Any(rp => rp.RoleId == u.RoleId && permissionIds.Contains(rp.PermissionId))
				)
				.Select(u => new ApplicationUserShortDetailsDto
				{
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					Email = u.Email
				})
				.ToListAsync();
		}
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
			passwordHasher.HashPassword(user, password);
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success;
        }
		public async Task<string> GetDatabaseName()
        {
            return ClientDbContext.Database.GetDbConnection().Database;
        }
		public async Task<bool> ConfirmAccount(string code, string password, ApplicationUser user) {
            var appUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.ConfirmationCode == code && u.Status == 0);
            if (appUser == null) throw new Exception("User not found.");

            var passwordHasher = new PasswordHasher<DataStore.Client.Models.User>();

            appUser.Password = passwordHasher.HashPassword(user, password);
			appUser.Status = (int)UserStatus.Confirmed;
            appUser.ConfirmationCode = null;

            await ClientDbContext.SaveChangesAsync();
            return true;
		}
		public async Task<ApplicationUserDto> UpdateAccountAsync(UserAccountPayload payload)
		{
			var user = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == payload.Id) ?? throw new Exception("User not found.");
			user.FirstName = payload.FirstName;
			user.LastName = payload.LastName;
			user.Phone = payload.Phone;
			user.AvatarUrl = payload.RemoveAvatar ? null : payload.AvatarUrl;

			await ClientDbContext.SaveChangesAsync();

			return this.mapper.Map<ApplicationUserDto>(user);

		}
		public async Task<ApplicationUserDto> ChangePasswordAsync(UserAccountChangePasswordPayload payload)
		{
			if (payload.Password.Trim() != payload.ConfirmPassword.Trim()) throw new Exception("Passwords don't match.");

			var user = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == payload.Id) ?? throw new Exception("User not found.");

			var passwordHasher = new PasswordHasher<ApplicationUser>();
			var appUser = mapper.Map<ApplicationUser>(user);

			user.Password = passwordHasher.HashPassword(appUser, payload.Password);
			await ClientDbContext.SaveChangesAsync();

			return this.mapper.Map<ApplicationUserDto>(user);
		}
		public async Task<ApplicationUserDto> ChangeEmailAddressAsync(UserAccountChangeEmailPayload payload)
		{
			var user = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Id == payload.Id) ?? throw new Exception("User not found.");
			var applicationUser = mapper.Map<ApplicationUser>(user);
			var otherUserWithEmail = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Email == payload.NewEmail && u.Id != payload.Id);
			
			if (otherUserWithEmail != null)
				throw new Exception("The email address is already in use by another account.");

			var checkPassword = await this.CheckPasswordAsync(applicationUser, payload.Password);

			if (!checkPassword)
				throw new Exception("The provided password is incorrect.");

			user.Email = payload.NewEmail;

			await ClientDbContext.SaveChangesAsync();

			return this.mapper.Map<ApplicationUserDto>(user);
		}

		#region Private Methods
		private string GenerateResetPasswordEmailBody(string resetLink)
		{
			var uri = new Uri(resetLink);
			var siteDomainUrl = uri.Scheme + "://" + uri.Host;
			var emailBody = new StringBuilder();

			emailBody.Append("<html>");
			emailBody.Append("    <head>");
			emailBody.Append("        <style>html,body { padding: 0; margin:0; }</style>");
			emailBody.Append("    </head>");
			emailBody.Append("    <body>");
			emailBody.Append("        <div style=\"font-family:Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: "
									+ "15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#181c32!important\">");
			emailBody.Append("            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse;"
										+ "margin:0 auto; padding:0; max-width:600px\">");
			emailBody.Append("                <tbody>");
			emailBody.Append("                    <tr>");
			emailBody.Append("                        <td align=\"center\" valign=\"center\" style=\"text-align:center; padding: 40px\">");
			emailBody.Append($"                            <a href=\"{siteDomainUrl}\" rel=\"noopener\" target=\"_blank\">");
			emailBody.Append($"                                <img alt=\"Logo\" src=\"{siteDomainUrl}/assets/media/logos/6.png\" style=\"width: 269px;\" />");
			emailBody.Append("                            </a>");
			emailBody.Append("                        </td>");
			emailBody.Append("                    </tr>");
			emailBody.Append("                    <tr>");
			emailBody.Append("                        <td align=\"left\" valign=\"center\">");
			emailBody.Append("                            <div style=\"text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px\">");
			emailBody.Append("                                <div style=\"padding-bottom: 30px; font-size: 17px;\">");
			emailBody.Append("                                    <strong>Hello!</strong>");
			emailBody.Append("                                </div>");
			emailBody.Append("                                <div style=\"padding-bottom: 30px\">You are receiving this email because we received a password reset "
															+ "request for your account. To proceed with the password reset please click on the button below:</div>");
			emailBody.Append("                                <div style=\"padding-bottom: 40px; text-align:center;\">");
			emailBody.Append($"                                    <a href=\"{resetLink}\" rel=\"noopener\" style=\"text-decoration:none;display:inline-block;"
																+ $"text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;"
																+ $"color:#ffffff;background-color:#009EF7;border:0px;margin-right:0.75rem!important;font-weight:600!important;"
																+ $"outline:none!important;vertical-align:middle\" target=\"_blank\">Reset Password</a>");
			emailBody.Append("                                </div>");
			emailBody.Append("                                <div style=\"padding-bottom: 30px\">This password reset link will expire in 60 minutes. If you did not request a "
															+ "password reset, no further action is required.</div>");
			emailBody.Append("                                <div style=\"border-bottom: 1px solid #eeeeee; margin: 15px 0\"></div>");
			emailBody.Append("                                <div style=\"padding-bottom: 10px\">Kind regards,");
			emailBody.Append("                                <br>ContractorDesk.");
			emailBody.Append("                                <tr>");
			emailBody.Append("                                    <td align=\"center\" valign=\"center\" style=\"font-size: 13px; text-align:center;padding: 20px; color: #6d6e7c;\">");
			emailBody.Append("                                        <p>Copyright ©");
			emailBody.Append($"                                        <a href=\"{siteDomainUrl}\" rel=\"noopener\" target=\"_blank\">ContractorDesk</a>.</p>");
			emailBody.Append("                                    </td>");
			emailBody.Append("                            </div>");
			emailBody.Append("                        </td>");
			emailBody.Append("                    </tr>");
			emailBody.Append("                </tbody>");
			emailBody.Append("            </table>");
			emailBody.Append("        </div>");
			emailBody.Append("    </body>");
			emailBody.Append("</html>");

			return emailBody.ToString();
		}

		#endregion
	}
}

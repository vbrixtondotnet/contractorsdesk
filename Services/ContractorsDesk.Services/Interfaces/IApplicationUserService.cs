using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
namespace ContractorsDesk.Services.Interfaces
{
	public interface IApplicationUserService : IBaseService
	{
		Task<List<ApplicationUserShortDetailsDto>> SearchUserAsync(string? key);
		Task<List<ApplicationUserShortDetailsDto>> GetAllSupervisorsAsync();
		Task<List<ApplicationUserDto>> GetUsersByRoleCategoryAsync(int roleCategoryId);
		Task<List<ApplicationUserDto>> GetUsersByCompanyAsync(int? companyId);
        Task<List<ApplicationUserDto>> GetUsersByRoleAsync(int roleId);
		Task<ApplicationUserDto> GetUserByIdAsync(int id);
		Task<AccountDetailsDto> GetAccountDetailsAsync(int id);
		Task<ApplicationUser> GetUserByEmailAddressAsync(string emailAddress);
        Task<bool> SetLogOnRequirementAsync(int id, bool value);
		Task UserResetPasswordRequest(UserResetPasswordDto passwordRequestDto);
		Task<Tuple<bool, Dictionary<string, string>>> UserResetPassword(ApplicationUser user, UserResetPasswordDto passwordRequestDto);
		Task<List<ApplicationUserDto>> GetUsersByManageAllJobsPermission();
		Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
		Task<string> GetDatabaseName();
        Task<ApplicationUser> GetUserByConfirmationCodeAsync(string code);
		Task<bool> ConfirmAccount(string code, string password, ApplicationUser user);
		Task<ApplicationUserDto> UpdateAccountAsync(UserAccountPayload payload);
		Task<ApplicationUserDto> ChangePasswordAsync(UserAccountChangePasswordPayload payload);
		Task<ApplicationUserDto> ChangeEmailAddressAsync(UserAccountChangeEmailPayload payload);
	}
}

using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.WebPortal.Models
{
	public class ApplicationUserModel
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public int RoleId { get; set; }
		public int RoleCategoryId {  get; set; }
		public int? CompanyId { get; set; }

		private string? _avatarUrl;
		public string? AvatarUrl
		{
			get
			{
				return this._avatarUrl ?? "/assets/media/avatars/blank.png";
			}
			set { this._avatarUrl = value; }
		}
		public List<PermissionDto> Permissions { get; set; }
    }
}

using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class AccountDetailsDto
	{
		public int Id { get; set; }

		public string FirstName { get; set; } = null!;

		public string LastName { get; set; } = null!;

		public string Email { get; set; } = null!;

		public string? Phone { get; set; }

		private string? _avatarUrl;
		public string? AvatarUrl
		{
			get
			{
				return this._avatarUrl ?? "/assets/media/avatars/blank.png";
			}
			set { this._avatarUrl = value; }
		}

		private string _password;
		public string Password
		{
			get
			{
				return this._password.Substring(0, 15);
			}
			set { this._password = value; }
		}

		public int RoleId { get; set; }

		public string Initials
		{
			get
			{
				var firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "";
				var lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString().ToUpper() : "";
				return firstInitial + lastInitial;
			}
		}

		public string Role
		{
			get
			{
				return ((Roles)RoleId).GetStringValue();
			}
		}
	}
}

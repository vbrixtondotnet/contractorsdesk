using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class UserResetPasswordDto
	{
		public Guid? Id { get; set; }

		public string Email { get; set; }

		public DateTime DateSent { get; set; }

		public bool? IsRecovered { get; set; }

		public string? SentStatus { get; set; }

		public string? ResetLink { get; set; }

		public string? Token { get; set; }

		public string? Password { get; set; }

		public string? ConfirmPassword { get; set; }
	}
}

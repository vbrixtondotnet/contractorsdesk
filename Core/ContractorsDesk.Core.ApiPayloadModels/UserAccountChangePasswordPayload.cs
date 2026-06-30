using ContractorsDesk.Core.ApiPayloadModels.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UserAccountChangePasswordPayload : BaseModel
	{
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
	}

	public class UserAccountChangeEmailPayload : BaseModel
	{
		public string NewEmail { get; set; }
		public string Password { get; set; }
	}
}

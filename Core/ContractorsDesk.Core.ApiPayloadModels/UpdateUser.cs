using ContractorsDesk.Core.ApiPayloadModels.@base;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UpdateUser : BaseModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int RoleId { get; set; }
	}
}

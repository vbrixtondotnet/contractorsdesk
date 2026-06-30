using ContractorsDesk.Core.ApiPayloadModels.@base;
using System.Text.Json.Serialization;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UserPayload : BaseModel
	{
		public string FirstName {  get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string? Password { get; set; }
		public string? ConfirmPassword { get; set; }
		public int RoleId { get; set; }
		public bool IsDeleted { get; set; } = false;
		public string? ConfirmationCode { get; set; }
		public int Status { get; set; }
	}
}

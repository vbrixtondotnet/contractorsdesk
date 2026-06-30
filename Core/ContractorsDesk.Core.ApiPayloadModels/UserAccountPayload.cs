using ContractorsDesk.Core.ApiPayloadModels.@base;
using System.Text.Json.Serialization;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UserAccountPayload : BaseModel
	{
		public string FirstName {  get; set; }
		public string LastName { get; set; }
		public string? Phone { get; set; }
		public string? AvatarFileName { get; set; }
		public string? AvatarBase64 { get; set; }
		public string? AvatarUrl { get; set; }
		public bool RemoveAvatar { get; set; } = false;
	}
}

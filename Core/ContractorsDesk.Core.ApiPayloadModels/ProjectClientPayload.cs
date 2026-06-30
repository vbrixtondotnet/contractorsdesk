using ContractorsDesk.Core.ApiPayloadModels.@base;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ProjectClientPayload : BaseModel
	{
		public Guid CustomerId { get; set; }
		public string Name { get; set; } = null!;
		public string? FullName { get; set; }
		public string? CompanyName { get; set; } = null!;
		public string? Address { get; set; } = null!;
		public string? Phone { get; set; } = null!;
		public string Email { get; set; } = null!;
        public string? SecondaryEmail { get; set; } = null!;
        public string? City { get; set; } = null!;
		public string? State { get; set; } = null!;
	}
}

using ContractorsDesk.Core.Enums;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalClientDto
	{
		public Guid Id { get; set; }

		public Guid ProposalId { get; set; }

		public string? FirstName { get; set; } = string.Empty;

		public string? LastName { get; set; } = string.Empty;

		public string? CompanyName { get; set; }

		public string? Address { get; set; }

		public string? Phone { get; set; }

		public string? Email { get; set; }

		public string? City { get; set; }

		public string? State {get;set;}
	}
}

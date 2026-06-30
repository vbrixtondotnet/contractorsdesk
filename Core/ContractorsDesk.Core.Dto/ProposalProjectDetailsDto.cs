namespace ContractorsDesk.Core.Dto
{
	public class ProposalProjectDetailsDto
	{
		public Guid Id { get; set; }
		public Guid? ProposalId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public string? Description { get; set; } = string.Empty;
		public string? City { get; set; } = string.Empty;
		public string? State { get; set; } = string.Empty;
		public decimal? SqFeet { get; set; }
    }
}

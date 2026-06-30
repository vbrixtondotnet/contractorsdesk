namespace ContractorsDesk.Core.Dto
{
	public class ProposalTemplateDto
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public bool IsDefault { get; set; }
		public int Sequence { get; set; }
		public List<ProposalTemplateLineItemDto> Categories { get; set; } = new List<ProposalTemplateLineItemDto>();
		public virtual List<ApplicationUserShortDetailsDto> Owner { get; set; } = new List<ApplicationUserShortDetailsDto>();
		public bool CanBeDeleted { get; set; }
        public DateTime? DateCreated { get; set; }

    }
}

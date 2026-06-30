namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ProposalTemplatePayload
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool Default { get; set; } = false;
		public List<ProposalTemplateCategoryPayload> LineItems { get; set; }
	}

	public class ProposalTemplateCategoryPayload
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Sequence { get; set; }
		public List<ProposalTemplateItemPayload> LineItems { get; set; }
	}

	public class ProposalTemplateItemPayload
	{
		public Guid Id { get; set; }
		public Guid ProposalTemplateId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public int Sequence { get; set; }
		public Guid ParentId { get; set; }
		public int? Percentage { get; set; }
		public Guid? EstimateCategoryId { get; set; }
	}
}

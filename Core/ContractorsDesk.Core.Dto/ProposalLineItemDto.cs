namespace ContractorsDesk.Core.Dto
{
	public class ProposalLineItemDto
	{
		public Guid? CategoryId { get; set; }
		public int Sequence { get; set; }
		public string? CategoryName { get; set; }
		public Guid ItemId { get; set; }
		public int? ItemSequence { get; set; }
		public string ItemName { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public decimal? Percentage { get;set; }
		public decimal? SqFoot { get; set; }
		public decimal? SqFootLocked { get; set; }

		public decimal? Multiplier { get; set; }

	}

	public class ProposalLineItemShortDto
	{
		public Guid EstimateCategoryId { get; set; }
		public string? Name { get; set; }
		public decimal? Amount { get; set; }
	}
}

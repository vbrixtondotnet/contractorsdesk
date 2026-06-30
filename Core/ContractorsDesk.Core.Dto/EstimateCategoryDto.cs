namespace ContractorsDesk.Core.Dto
{
	public class EstimateCategoryDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public int Sequence { get; set; }

		public Guid? ParentEstimateCategoryId { get; set; }
		public EstimateCategoryDto? Parent { get; set; }

		public DateTime? Created { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? Updated { get; set; }

		public string? UpdatedBy { get; set; }

		public string? Description { get; set; }
	}
	public class EstimateCategoryShortDetailsDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public Guid? ParentEstimateCategoryId { get; set; }
		public decimal Amount { get; set; }
		public string Parent { get; set; } = null!;
		public int? ParentSequence { get; set; }
		public bool Deleted { get; set; }

	}
}

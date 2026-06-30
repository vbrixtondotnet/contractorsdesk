namespace ContractorsDesk.Core.Dto
{
	public class EstimateHistoryDto
	{
		public Guid Id { get; set; }

		public decimal? Amount { get; set; }

		public Guid? EstimateId { get; set; }

		public DateTime? Created { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? Updated { get; set; }

		public string? UpdatedBy { get; set; }

		public Guid? QbclassId { get; set; }

	}
}

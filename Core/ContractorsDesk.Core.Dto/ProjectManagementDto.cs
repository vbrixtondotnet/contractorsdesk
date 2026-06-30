namespace ContractorsDesk.Core.Dto
{
	public class ProjectManagementDto
	{
		public Guid ProposalId { get; set; }

		public Guid Id { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string Status { get; set; } = null!;

		public Guid? AssignedTo { get; set; }

		public decimal ProgressPercentage { get; set; }

		public string Notes { get; set; } = null!;

		public string Description { get; set; } = null!;

		public DateTime? Created { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime Date { get; set; }

		public int Number { get; set; }

		public DateTime? Updated { get; set; }

		public string? UpdatedBy { get; set; }
	}
}

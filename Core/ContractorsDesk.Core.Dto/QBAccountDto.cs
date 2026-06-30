namespace ContractorsDesk.Core.Dto
{
	public class QBAccountDto
	{
		public Guid Id { get; set; }

		public string? ListId { get; set; }

		public string? Name { get; set; }

		public string? AccountNumber { get; set; }

		public string? ParentId { get; set; }

		public string? AccountType { get; set; }

		public DateTime? TimeCreated { get; set; }

		public DateTime? TimeModified { get; set; }

		public string? CreatedBy { get; set; }

		public string? UpdatedBy { get; set; }

		public string? FullyQualifiedName { get; set; }

		public bool? IsSubAccount { get; set; }

		public string? DetailType { get; set; }
	}
}

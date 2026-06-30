namespace ContractorsDesk.Core.Dto
{
	public class ProjectClientDto
	{
		public int Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid CustomerId { get; set; }
		public string? Name { get; set; } = null!;
		public string? FullName { get; set; }
		public string? CompanyName { get; set; } = null!;
		public string? Address { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? SecondaryEmail { get; set; } = null;
        public string DateCreated { get; set; }
		public int CreatedBy { get; set; }
		public string? DateUpdated { get; set; }
		public int? UpdatedBy { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
	}
}

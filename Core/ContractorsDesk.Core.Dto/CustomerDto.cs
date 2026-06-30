namespace ContractorsDesk.Core.Dto
{
	public class CustomerDto
	{
		public Guid Id { get; set; }

		public string? Name { get; set; }

		public string? FullName { get; set; }

		public string? CompanyName { get; set; }

		public string? Address { get; set; }

		public string? City { get; set; }

		public string? State { get; set; }

		public string? Currency { get; set; }

		public string? Phone { get; set; }

		public string? Email { get; set; }

        public string? SecondaryEmail { get; set; }

        public bool? IsActive { get; set; }

	}
}

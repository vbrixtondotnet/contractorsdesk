namespace ContractorsDesk.Core.Dto
{
	public class CustomerShortDetailsDto
	{
		public Guid Id { get; set; }

		public string FirstName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.FullName))
				{
					string[] parts = this.FullName.Split(new[] { ' ' }, 2);
					return parts[0];
				}
				return string.Empty;
			}
		}
		public string LastName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.FullName))
				{
					string[] parts = this.FullName.Split(new[] { ' ' }, 2);
					return parts.Length > 1 ? parts[1] : string.Empty;
				}

				return string.Empty;
			}
		}
		public string? Name { get; set; }
		public string? FullName { get; set; }

		public string? CompanyName { get; set; }

		public string? Address { get; set; }

		public string? Phone { get; set; }

		public string? Email { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? City { get; set; }

		public string? State { get; set; }

	}
	public class QbCustomerDto
	{
		public Guid Id { get; set; }

		public string ListId { get; set; } = null!;

		public string? Name { get; set; }

		public string? FullName { get; set; }

		public string? CompanyName { get; set; }

		public string? Address { get; set; }

		public string? Nace { get; set; }

		public decimal? Balance { get; set; }

		public string? Currency { get; set; }

		public string? Phone { get; set; }

		public string? Email { get; set; }

        public string? SecondaryEmail { get; set; }
        public bool? IsActive { get; set; }

		public int? Level { get; set; }

		public string? ParentId { get; set; }

		public string? JobStatus { get; set; }

		public DateTime? JobStartDate { get; set; }

		public DateTime? JobProjectedEndDate { get; set; }

		public DateTime? JobEndDate { get; set; }

		public string? JobDesc { get; set; }

		public string? Vatnumber { get; set; }

		public string? Nuinumber { get; set; }

		public DateTime? TimeCreated { get; set; }

		public DateTime? TimeModified { get; set; }

		public string? CreatedBy { get; set; }

		public string? UpdatedBy { get; set; }
	}
}

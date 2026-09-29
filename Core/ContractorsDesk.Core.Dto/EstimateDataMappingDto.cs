namespace ContractorsDesk.Core.Dto
{
	public class EstimateDataMappingPageDto
	{
		public List<EstimateDataMappingGroupDto> Groups { get; set; } = new();

		public List<EstimateMappingAccountOptionDto> Accounts { get; set; } = new();
	}

	public class EstimateDataMappingGroupDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public int Sequence { get; set; }

		public List<EstimateDataMappingItemDto> Items { get; set; } = new();
	}

	public class EstimateDataMappingItemDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public int Sequence { get; set; }

		public Guid ParentEstimateCategoryId { get; set; }

		public List<EstimateAccountMappingDto> Accounts { get; set; } = new();
	}

	public class EstimateAccountMappingDto
	{
		public Guid? MappingId { get; set; }

		public Guid AccountId { get; set; }

		public Guid EstimateCategoryId { get; set; }

		public string FullyQualifiedName { get; set; } = string.Empty;

		public bool Added { get; set; }

		public bool Removed { get; set; }
	}

	public class EstimateMappingAccountOptionDto
	{
		public Guid Id { get; set; }

		public string FullyQualifiedName { get; set; } = string.Empty;
	}
}

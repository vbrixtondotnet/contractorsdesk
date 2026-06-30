using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class EstimateDataMappingDto
	{
		public Guid? MappingId { get; set; }

		public Guid AccountId { get; set; }

		private string? _fullyQualifiedName;
		public string FullyQualifiedName
		{
			get => _fullyQualifiedName ?? string.Empty;
			set => _fullyQualifiedName = value;
		}

		public Guid? EstimateCategoryId { get; set; }

		private string? _estimateCategory;
		public string EstimateCategory
		{
			get => _estimateCategory ?? string.Empty;
			set => _estimateCategory = value;
		}

		private string? _parentCategory;
		public string ParentCategory
		{
			get => _parentCategory ?? string.Empty;
			set => _parentCategory = value;
		}

		public bool Updated { get;set; }
		public bool Added { get;set; }
	}
}

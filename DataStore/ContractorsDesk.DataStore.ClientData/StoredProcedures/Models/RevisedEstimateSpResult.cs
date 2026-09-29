using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class RevisedEstimateSpResult
	{
		public string Name { get; set; }
		public Guid? AccountId { get; set; }
		public Guid? EstimateCategoryId { get; set; }
		public Guid? ParentEstimateCategoryId { get; set; }
		public string? ParentEstimateCategory { get; set; }
		public decimal? OriginalAmount { get;set; }
		public decimal? RevisedAmount { get; set; }
		public decimal? CostToDate { get; set; }
		public decimal? Balance { get; set; }
		public int? ParentSequence { get; set; }
		public decimal? Percentage { get; set; }
		public int? Sequence { get; set; }
		public bool? HasEstimateMapping { get; set; }
	}
}

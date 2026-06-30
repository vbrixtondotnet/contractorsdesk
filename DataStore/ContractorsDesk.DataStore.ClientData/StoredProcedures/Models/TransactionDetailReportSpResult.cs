using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class TransactionDetailReportSpResult
	{
		public string? EstimateCategory {  get; set; }
		public string? EstimateSubCategory { get; set; }
		public Guid? EstimateCategoryId { get; set; }
		public Guid? ParentEstimateCategoryId { get; set; }
		public DateTime? Date { get;set; }
		public string? AccountType { get; set; }
		public string? Type { get; set; }
		public string? Num { get; set; }
		public string? Payee { get; set; }
		public string? Memo { get; set; }
		public decimal? Amount { get; set; }
		public int? CategorySequence {  get; set; }
		public int? ItemSequence { get; set; }
		public decimal? RevisedEstimate { get; set; }

	}
}

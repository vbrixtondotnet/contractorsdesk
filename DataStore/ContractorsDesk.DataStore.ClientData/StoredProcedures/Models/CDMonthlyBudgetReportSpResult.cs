using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
    public class CDMonthlyBudgetReportSpResult
	{
        public Guid ID { get; set; }
        public string? Name { get; set; }
		public decimal? TotalToDate { get; set; }
		public decimal? Estimate { get; set; }
		public decimal? RevisedEstimate { get; set; }
	}
}

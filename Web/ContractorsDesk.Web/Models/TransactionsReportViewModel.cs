using ContractorsDesk.DataStore.Client.StoredProcedures.Models;

namespace ContractorsDesk.WebPortal.Models
{
	public class TransactionsReportViewModel
	{
		public DateOnly StartDate {  get; set; }
		public DateOnly EndDate { get; set; }
		public List<TransactionsReportSpResult> Transactions {  get; set; }
	}
}

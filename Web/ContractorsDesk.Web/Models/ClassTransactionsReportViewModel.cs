using ContractorsDesk.DataStore.Client.StoredProcedures.Models;

namespace ContractorsDesk.WebPortal.Models
{
	public class ClassTransactionsReportViewModel
	{
		public DateOnly StartDate {  get; set; }
		public DateOnly EndDate { get; set; }
		public List<ClassTransactionsReportSpResult> ClassTransactions {  get; set; }
	}
}

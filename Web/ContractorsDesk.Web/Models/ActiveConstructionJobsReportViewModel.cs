using ContractorsDesk.DataStore.Client.StoredProcedures.Models;

namespace ContractorsDesk.WebPortal.Models
{
	public class ActiveConstructionJobsReportViewModel
	{
		public DateOnly StartDate {  get; set; }
		public DateOnly EndDate { get; set; }
		public List<ActiveConstructionJobSpResult> ActiveConstructionJobs {  get; set; }
	}
}

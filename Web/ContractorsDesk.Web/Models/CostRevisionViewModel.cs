using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.WebPortal.Models
{
	public class CostRevisionViewModel
	{
		public CostRevisionDto CostRevisionDto { get; set; }
		public string ClientName { get; set; }
		public string StartDate { get; set; }
		public string ProjectName { get; set; }

		public string ProjectAddress {  get; set; }

	}
}

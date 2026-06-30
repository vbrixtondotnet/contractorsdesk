using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.WebPortal.Models
{
	public class ChangeOrderFormViewModel
	{
		public string ChangeOrderType { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string? EstimateCategory { get; set; } = string.Empty;
		public decimal? Amount { get; set; }
		public decimal? CurrentAmount { get; set; }
		public decimal? NewAmount { get; set; }
		public string? ScheduleItemName { get; set; } = string.Empty;
		public int? NoOfDays { get; set; }
		public string ClientName { get; set; }
		public string  ProjectName { get; set; }
		public string ProjectAddress { get; set; } = string.Empty;
		public string ContractorName { get; set; } = string.Empty;
		public string ChangeOrderNumber { get; set; }
	}
}

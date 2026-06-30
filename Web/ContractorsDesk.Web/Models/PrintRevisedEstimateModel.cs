using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.WebPortal.Models
{
	public class PrintRevisedEstimateModel
	{
		public string ProjectName {  get; set; }
		public string StartDate {  get; set; }
		public string EstimatedCompletionDate {  get; set; }

		public decimal ProjectTotalOriginal
		{
			get
			{
				decimal retval = 0;
				foreach (var category in this.Categories)
				{
					retval += category.TotalOriginal;
				}
				return retval;
			}
		}
		public decimal ProjectTotalRevised {
			get
			{
				decimal retval = 0;
				foreach (var category in this.Categories)
				{
					retval += category.TotalRevised;
				}
				return retval;
			}
		}
		public decimal ProjectTotalCostToDate {
			get
			{
				decimal retval = 0;
				foreach (var category in this.Categories)
				{
					if (category.Name != "JOB BALANCE" && category.Name != "OWNER DEPOSITS")
					{
						retval += category.TotalCostToDate;
					}
				}
				return retval;
			}
		}
		public decimal ProjectTotalBalance {
			get
			{
				decimal retval = 0;
				foreach (var category in this.Categories)
				{
					retval += category.TotalBalance;
				}
				return retval;
			}
		}
		public decimal ProjectTotalPercent {
			get
			{
				return Math.Round((this.ProjectTotalCostToDate / this.ProjectTotalRevised) * 100);
			}
		}
		public decimal TotalOwnerDeposits
		{
			get
			{
				decimal retval = 0;
				var ownerDeposits = this.Categories.FirstOrDefault(c => c.Name == "OWNER DEPOSITS");

				return ownerDeposits != null ? ownerDeposits.TotalCostToDate : retval;
			}
		}
		public decimal TotalJobBalance
		{
			get
			{
				decimal retval = 0;
				var jobBalance = this.Categories.FirstOrDefault(c => c.Name == "JOB BALANCE");

				return jobBalance != null ? jobBalance.TotalCostToDate : retval;
			}
		}
		public decimal TotalMinimumRequestedAmount
		{
			get
			{
				decimal retval = 0;
				var amount = this.Categories.FirstOrDefault(c => c.Name == "MINIMUM REQUESTED AMOUNT");

				return amount != null ? amount.TotalCostToDate : retval;
			}
		}
		public List<RevisedEstimateCategoryDto> Categories { get; set; }
		public Summary Summary { get; set; }
    }
}

namespace ContractorsDesk.Core.Enums
{
	public enum EmailTypes
	{
		[StringValue("StatusReport")]
		StatusReport = 1,
		[StringValue("ClientEmail")]
		ClientEmail = 2,
		[StringValue("Schedule Report")]
		ScheduleReport = 3,
		[StringValue("Proposal Report")]
		ProposalReport = 4,
		[StringValue("Estimate to Actual Report")]
		EstimateToActualReport = 5,
		[StringValue("Request Deposit")]
		RequestDeposit = 6,
		[StringValue("Invoice")]
		Invoice = 7,
		[StringValue("Deposit Request")]
		DepositRequest = 8,
		[StringValue("Complete Action Item")]
		CompleteActionItem = 9,
		[StringValue("Schedule Revision")]
		ScheduleRevision = 10,
		[StringValue("Cost Revision")]
		CostRevision = 11,
        [StringValue("Change Order")]
        ChangeOrder = 12
    }
}

namespace ContractorsDesk.Core.Enums
{
	public enum NotificationMessage
	{
		[StringValue("Cost Change for {0} has been approved by {1}.")]
		CostChangeApproved = 1,
		[StringValue("New action item({0}) created and assigned to you.")]
		NewActionItemCreated = 2,
		[StringValue("Action item({0}) has been accepted.")]
		ActionItemAccepted = 3,
		[StringValue("New {0} has been processed by AI.")]
		ProcessedByAI = 4
	}
}

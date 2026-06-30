namespace ContractorsDesk.Core.Enums
{
	public enum ActionItemStatus
	{
		[StringValue("Not Started")]
		NotStarted = 1,
		[StringValue("In Progress")]
		InProgress = 2,
		[StringValue("For Review")]
		ForReview= 3,
		[StringValue("Pending Client Response")]
		PendingClientResponse = 4,
		[StringValue("Client Approved")]
		ClientApproved = 5,
		[StringValue("Completed")]
		Completed = 6,
		[StringValue("Archived")]
		Archived = 7,
        [StringValue("Pending Client Acknowledgement")]
        PendingClientAcknowledgement = 8,
		[StringValue("Client Acknowledged")]
		ClientAcknowledged = 9,
	}
}

namespace ContractorsDesk.Core.Enums
{
	public enum ActionTypes
	{
		[StringValue("Cost Change")]
		CostChange = 1,
		[StringValue("Schedule Change")]
		ScheduleChange = 2,
		[StringValue("Client Contact")]
		ClientContact = 3,
		[StringValue("Sub-contractor Contact")]
		SubContractorContact = 4,
		[StringValue("Note")]
		Note = 5,
		[StringValue("Follow up, Sub-contractor, Client, City, etc.")]
		Followup = 6,
		[StringValue("Reminder, alarm, notification text/email")]
		Reminder = 7,
        [StringValue("GeneralChangeOrder")]
        GeneralChangeOrder = 8
    }
}

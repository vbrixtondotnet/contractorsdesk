namespace ContractorsDesk.Core.Enums
{
	public enum DocumentType
	{
		[StringValue("Client Contract")]
		ClientContract = 1,
		[StringValue("Proposal")]
		Proposal = 2,
		[StringValue("Change Order")]
		ChangeOrder = 3,
		[StringValue("Invoice")]
		Invoice = 4,
		[StringValue("Receipt")]
		Receipt = 5,
		[StringValue("Payment")]
		Payment = 6,
		[StringValue("Estimate")]
		Estimate = 7,
		[StringValue("Other")]
		Other = 8
	}
}

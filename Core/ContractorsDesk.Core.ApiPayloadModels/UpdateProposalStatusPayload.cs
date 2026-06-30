namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UpdateProposalStatusPayload
	{
		public Guid ProposalId { get; set; }
		public string DocStatus { get; set; }
	}
}

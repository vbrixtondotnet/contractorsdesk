namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UpdateProposalArchivePayload
    {
		public Guid ProposalId { get; set; }
		public bool IsArchived { get; set; }
	}
}

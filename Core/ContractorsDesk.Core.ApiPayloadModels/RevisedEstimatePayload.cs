using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class RevisedEstimatePayload
	{
		public Guid ProposalId { get; set; }
		public List<RevisedEstimateCategoryDto> Categories { get;set; }
	}
}

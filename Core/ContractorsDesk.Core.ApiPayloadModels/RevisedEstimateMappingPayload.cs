using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class RevisedEstimateMappingPayload
	{
		public Guid ProposalId { get; set; }
		public List<RevisedEstimateMapping> Mappings { get; set; }
	}

	public class RevisedEstimateMapping
	{
		public Guid AccountId { get; set; }
		public Guid EstimateCategoryId { get; set; }
		public string? Name { get; set; }
		public string? Parent { get; set; }
		public decimal Amount { get; set; }
	}
}

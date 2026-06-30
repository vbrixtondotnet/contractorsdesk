using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ActionItemCostChangeDto
	{
		public int Id { get; set; }
		public int? ActionItemId { get; set; }
		public decimal Amount { get; set; }
		public decimal OriginalAmount { get; set; }
		public Guid? EstimateCategoryId { get; set; }
		public ActionItemDto ActionItem  { get; set; }
		public ProposalProjectDetailsDto Project { get; set; }
		public ProposalClientDto Client { get; set; }
		public string EstimateCategory { get; set; } = string.Empty;
		public bool? RequiresClientApproval { get; set; }
		public string? LineItem { get; set; }
	}
}

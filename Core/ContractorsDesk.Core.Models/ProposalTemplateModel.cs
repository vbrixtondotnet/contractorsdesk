using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class ProposalTemplateModel
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
		public bool IsDefault { get; set; }
		public List<ProposalTemplateLineItemModel> Categories { get; set; } = new List<ProposalTemplateLineItemModel>();
	}
}

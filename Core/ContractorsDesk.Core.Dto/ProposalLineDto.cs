using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalLineDto
	{
		public Guid EstimateCategoryId { get; set; }
		public Guid ParentEstimateCategoryId { get;set; }
		public string Name { get; set; }
		public string ParentName { get; set; }
		public int? ParentSequence { get; set; }
		public int? Sequence { get; set; }
	}
}

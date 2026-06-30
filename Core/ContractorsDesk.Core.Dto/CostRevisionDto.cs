using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class CostRevisionDto
	{
		public Guid Id { get; set; }

		public Guid ProjectId { get; set; }

		public int? ActionItemId { get; set; }

		public int RevisionNumber { get; set; }

		public DateTime RevisionDate { get; set; }

		public int StatusId { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int? UpdatedBy { get; set; }

		public List<CostRevisionItemDto> CostRevisionItems { get; set; } = new List<CostRevisionItemDto>();
	}
}

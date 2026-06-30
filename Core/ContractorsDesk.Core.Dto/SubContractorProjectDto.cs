using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class SubContractorProjectDto
    {
		public Guid Id { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int? UpdatedBy { get; set; }

		public List<ProjectDetailsDto>? Projects { get; set; }
	}
}

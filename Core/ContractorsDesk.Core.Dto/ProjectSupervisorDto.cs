using ContractorsDesk.Core.Dto.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProjectSupervisorDto : BaseDto
	{
		public Guid ProjectId { get; set; }

		public int SupervisorId { get; set; }

		public DateOnly DateAssigned { get; set; }

		public int? SupervisorTypeId { get; set; }

		public ApplicationUserShortDetailsDto Supervisor { get; set; }
	}
}

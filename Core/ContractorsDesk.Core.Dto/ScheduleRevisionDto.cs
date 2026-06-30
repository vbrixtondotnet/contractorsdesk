using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ScheduleRevisionDto
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
		public DateOnly StartDate { get; set; }
		public string ProjectName { get; set; }
		public string ProjectAddress { get; set; }
		public string ClientName { get; set; }
		public List<ScheduleRevisionItemDto> ScheduleRevisionItems { get; set; } = new List<ScheduleRevisionItemDto>();
	}
}

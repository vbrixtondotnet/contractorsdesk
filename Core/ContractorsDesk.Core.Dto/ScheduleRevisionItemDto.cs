using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ScheduleRevisionItemDto
	{
		public Guid Id { get; set; }

		public string Reason { get; set; } = null!;

		public string? Description { get; set; }

		public Guid ConstructionTaskId { get; set; }

		public int OldDuration { get; set; }

		public int NewDuration { get; set; }

		public DateOnly OldStartDate { get; set; }

		public DateOnly NewStartDate { get; set; }

		public DateOnly OldEndDate { get; set; }

		public DateOnly NewEndDate { get; set; }

		public string ConstructionTask { get; set; }
	}
}

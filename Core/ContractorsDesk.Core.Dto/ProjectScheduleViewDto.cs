using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProjectScheduleViewDto
	{
		public ProjectDetailsDto ProjectDetails { get; set; }
		public string Status { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EstimatedCompletionDate { get; set; }
		public List<ProjectScheduleTaskDto> Tasks { get; set; }
		public List<ProjectScheduleDelayDto> Delays { get; set; } = new List<ProjectScheduleDelayDto>();
		public ScheduleRevisionDto? ScheduleRevision { get; set; } = null;
		public List<ProjectScheduleDelayUpdateDto> UpdatedDelays { get;set; } = new List<ProjectScheduleDelayUpdateDto>();
	}
}

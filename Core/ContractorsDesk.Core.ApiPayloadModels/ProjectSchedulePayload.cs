using ContractorsDesk.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ProjectSchedulePayload
	{
		public Guid? Id { get; set; }
		public Guid ProjectId { get; set; }

		private string? _projectName;
		public string? ProjectName
		{
			get => this._projectName ?? string.Empty;
			set => this._projectName = value;
		}
		public string Status { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EstimatedCompletionDate { get; set; }
		public List<ProjectScheduleTaskDto> Tasks { get; set; }
		public List<ProjectScheduleDelayDto> Delays { get; set; } = new List<ProjectScheduleDelayDto>();
		public List<ScheduleRevisionPayload> ScheduleRevisions { get; set; } = new List<ScheduleRevisionPayload>();
		public List<ProjectScheduleDelayUpdateDto> UpdatedDelays { get; set; } = new List<ProjectScheduleDelayUpdateDto>();
		public bool? IncludeClientDocument { get; set; }
	}

	public class ScheduleRevisionPayload
	{
		public int? ActionItemId { get; set; }

		public string Reason { get; set; } = null!;

		public string? Description { get; set; }

		public Guid ConstructionTaskId { get; set; }

		public int NewDuration { get; set; }

		public DateOnly NewStartDate { get; set; }

		public DateOnly NewEndDate { get; set; }
	}
}

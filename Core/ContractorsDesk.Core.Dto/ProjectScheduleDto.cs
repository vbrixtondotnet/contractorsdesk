namespace ContractorsDesk.Core.Dto
{
	public class ProjectScheduleDto
	{
		public Guid? Id { get; set; }
		public Guid ProjectId { get; set; }

		private string? _projectName;
		public string? ProjectName {
			get => this._projectName ?? string.Empty;
			set => this._projectName = value; 
		}
		public string Status { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EstimatedCompletionDate { get; set; }
		public List<ProjectScheduleTaskDto> Tasks { get; set; }
		public List<ProjectScheduleDelayDto> Delays { get; set; } = new List<ProjectScheduleDelayDto>();
		public bool? IncludeClientDocument { get; set; }

	}
}
namespace ContractorsDesk.Core.Dto
{
	public class ProjectScheduleDelayDto
	{
		public Guid Id { get; set; }

		public DateTime Start { get; set; }

		public string Reason { get; set; } = null!;

		public string TaskName
		{
			get
			{
				return this.Task != null ? this.Task.Name : string.Empty;
			}
		}

		public string? Description { get; set; }

		public int Days { get; set; }

		public bool? ApplyToOtherProjects { get; set; }

		public Guid? TaskId { get; set; }

        public ProjectScheduleTaskDto? Task { get; set; }

		public bool New { get; set; } = false;
    }
}

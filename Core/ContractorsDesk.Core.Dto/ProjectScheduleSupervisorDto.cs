
namespace ContractorsDesk.Core.Dto
{
	public class ProjectScheduleSupervisorDto
	{
		public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ProjectScheduleId { get; set; }
		public int? ProjectSupervisorId { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is not ProjectScheduleSupervisorDto other) return false;
			return ProjectId == other.ProjectId
				&& ProjectScheduleId == other.ProjectScheduleId
				&& ProjectSupervisorId == other.ProjectSupervisorId;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ProjectId, ProjectScheduleId, ProjectSupervisorId);
		}
	}
}

namespace ContractorsDesk.Core.Dto
{
	public class ScheduleDataWithTaskMappingDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Sequence { get; set; }
		public Guid? ParentEstimateCategoryId { get; set; }
        public string? ParentEstimateCategory { get; set; }
        public DateTime Created { get; set; }
		public int CreatedBy { get; set; }
		public DateTime? Updated { get; set; }
		public int? UpdatedBy { get; set; }
		public string? Description { get; set; }
		public List<TaskMappingDto>? Tasks { get; set; }
    }

	public class TaskMappingDto
	{
		public Guid Id { get; set; }
		public Guid ConstructionTaskId { get; set; }
		public Guid EstimateCategoryId { get; set; }
		public string? Name { get; set; }
		public bool Added { get; set; }
		public bool Updated { get; set; }
	}
}

using ContractorsDesk.Core.ApiPayloadModels.@base;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ActionItemPayload : BaseModel
	{
		public string Title { get; set; }
		public string? Description { get; set; }
		public Guid? ProjectId { get; set; }
		public int ActionTypeId { get; set; }
		public int Source { get; set; } = 1; // Default to 1 (Internal)
		public List<int> Supervisors { get; set; }
		public DateOnly DueDate { get; set; }
		public decimal? CostChangeAmount { get; set; }
		public Guid? CostChangeEstimateCategoryId { get; set; }
		public bool? CostChangeRequiresClientApproval { get; set; }
		public int? ScheduleChangeNumberOfDays { get; set; }
		public Guid? ScheduleChangeTaskId{ get; set; }
		public bool? ScheduleChangeRequiresClientApproval { get; set; }
		public bool StartOnSaveChanges { get; set; } = false; 
		public bool IsAiCreated { get; set; } = false; // Default to false, indicating it's not a draft
		public int Status { get; set; }
	}
}

using ContractorsDesk.Core.Dto.@base;
using ContractorsDesk.Core.Enums;
namespace ContractorsDesk.Core.Dto
{
	public class ActionItemDto : BaseDto
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public Guid ProjectId { get; set; }
		public Guid ProposalId { get; set; }
		public int ActionTypeId { get; set; }
		public int ProjectManagerId { get; set; }
		public string ProjectManagerEmail { get; set; } = string.Empty;
		public string ProjectManagerName { get; set; } = string.Empty;
		public string Status { get; set; }
		public DateOnly DueDate { get; set; }
		public string ProjectName { get; set; }
		public string ActionTypeName { get; set; }
		private int statusId { get; set; }
		public bool IsArchived { get; set; }
		public DateTime DateCreated { get; set; }
		public int StatusId
		{
			get { return this.statusId; }
			set
			{
				this.statusId = value;
				this.Status = ((ActionItemStatus)(this.statusId)).GetStringValue();
			}
		}
		public int Source { get; set; }
		public CustomerDto? ProjectClient { get; set; }
		public ActionItemCostChangeDto CostChange { get; set; }
		public ActionItemScheduleChangeDto ScheduleChange { get; set; }
		public ApplicationUserShortDetailsDto? CreatedBy { get; set; }
		public List<ApplicationUserShortDetailsDto>? Supervisors { get; set; }
		public ApplicationUserShortDetailsDto? AcceptedBy { get; set; }
	}
}

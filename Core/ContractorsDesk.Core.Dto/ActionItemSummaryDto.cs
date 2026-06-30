using ContractorsDesk.Core.Dto.@base;
using ContractorsDesk.Core.Enums;
namespace ContractorsDesk.Core.Dto
{
	public class ActionItemSummaryDto : BaseDto
	{
		public Guid ProjectId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public DateTime? DateCreated { get; set; }
		public DateOnly? DueDate { get; set; }
		public int ActionTypeId { get; set; }
		public string ActionTypeName { get; set; }
		private int statusId { get; set; }
		public decimal? CurrentAmount { get; set; }
		public decimal? CostChangeAmount { get; set; }
		public decimal? NewRevisedTotal
		{
			get
			{

				if (this.CostChangeAmount != null || this.CurrentAmount != null)
				{
					return this.CurrentAmount + this.CostChangeAmount;
				}
				return null;
			}
		}
		public Guid? LineItemid { get; set; }
		public Guid? ConstructionTaskId { get; set; }
		public string LineItemName { get; set; } = string.Empty;
		public string ConstructionTask { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public int? OriginalDuration { get; set; }
		public int? NoOfDaysChange { get; set; }
		public int? NewDuration { get; set; }
		public ApplicationUserShortDetailsDto? CreatedBy { get; set; }
		public ProposalLineItemShortDto? ProposalLineItem { get; set; } = null;
		public bool RequiresClientApproval { get; set; }
		public string? ProjectName { get; set; }
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
		public List<ApplicationUserShortDetailsDto> AssignedSupervisors { get; set; } = [];
	}
}

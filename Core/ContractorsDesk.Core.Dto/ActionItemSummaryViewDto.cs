using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ActionItemSummaryViewDto
	{
		public int Id { get; set; }

		public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int ActionTypeId { get; set; }

		public string ActionTypeName { get; set; } = null!;
		public DateOnly DueDate { get; set; }

		public Guid? ProposalId { get; set; }

		public Guid? ProjectId { get; set; }

		public string? ProjectName { get; set; }

		public int StatusId { get; set; }

        public bool IsArchived { get; set; }

        public DateTime DateCreated { get; set; }

		public Guid? CostChangeItemId { get; set; }

		public string? CostChangeItem { get; set; }

		public decimal? Amount { get; set; }

		public Guid? ScheduleChangeItemId { get; set; }

		public string? ScheduleChangeItem { get; set; }

		public int? NoOfDays { get; set; }
		public int SupervisorId { get; set; }

		public decimal? CurrentAmount { get; set; }

		public int? Duration { get; set; }

		public decimal? RevisedAmount
		{
			get
			{
				if(this.CurrentAmount != null || this.Amount != null)
				{
					return this.CurrentAmount + this.Amount;
				}
				return null;
			}
		}

		public int? NewDuration
		{
			get
			{
				if(this.Duration != null || this.NoOfDays != null)
				{
					return this.Duration + this.NoOfDays;
				}

				return null;
			}
		}
		public string Status
		{
			get
			{
				return ((ActionItemStatus)StatusId).GetStringValue();
			}
		}
		public int Source { get; set; }
		public string? CreatedBy { get; set; }
		public int CreatedById { get; set; }
		public ApplicationUserShortDetailsDto? CreatedByUser { get; set; } = null;
		public List<ApplicationUserShortDetailsDto> AssignedSupervisors { get; set; } = [];
	}
}

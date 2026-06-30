using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ActionItemShortDetailsDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public int ActionTypeId { get; set; }
		public DateOnly DueDate { get; set; }
		public string? ProjectName { get; set; }
		public int StatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public string ActionTypeName
		{
			get
			{
				return ((ActionTypes)ActionTypeId).GetStringValue();
			}
		}
		public string Status
		{
			get
			{
				return ((ActionItemStatus)StatusId).GetStringValue();
			}
		}
		public ApplicationUserShortDetailsDto CreatedByUser { get; set; }
		public List<ApplicationUserShortDetailsDto> AssignedSupervisors { get; set; } = [];
		public List<ActionItemCommentDto> Comments { get; set; } = [];
	}
}

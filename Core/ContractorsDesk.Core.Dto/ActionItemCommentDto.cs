using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ActionItemCommentDto
	{
		public int Id { get; set; }

		public int ActionItemId { get; set; }

		public string? Comment { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime? DateUpdated { get; set; }

		public ApplicationUserShortDetailsDto? CreatedBy { get; set; }
	}
}

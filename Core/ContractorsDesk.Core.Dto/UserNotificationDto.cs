using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class UserNotificationDto
	{
		public Guid? Id { get; set; }

		public string? Prefix { get; set; }

		public string? Message { get; set; }

		public string? Description { get; set; }

		public string? CreatedBy { get; set; }

		public string? CreatedByInitials { get; set; }

		public string? DateCreated { get; set; }

		public string? RelatedUrl { get; set; }

		public Guid? EmailId { get; set; }

		public bool? IsRead { get; set; }
	}
}

using ContractorsDesk.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class SentEmailDto
	{
		public Guid Id { get; set; }
		public Guid? ProjectId { get; set; }
		public string? ProjectName { get; set; }
		public string? Subject { get; set; } = null!;
		public string? To { get; set; }

		public string? Body { get; set; }

		public string? ReplyTo { get; set; }

		public string? Cc { get; set; }

		public string? Bcc { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public bool IsRead { get; set; }
		public int? EmailType { get; set; }

		public string? SentAt { get { return TimezoneUtils.CalculateTimeSinceEmailSent(this.DateCreated); } }

		public List<EmailAttachmentDto>? Attachments { get; set; }
	}
}

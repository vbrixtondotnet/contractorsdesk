using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class EmailPayloadModel
	{
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string? Cc { get; set; }
		public string? Bcc { get; set; }
		public EmailTypes? Type { get; set; }
		public Guid? RefId { get; set; }
		public int? Ref2Id { get; set; }
		public Guid? AdditionalRefId { get; set; }
		public Guid? ClientId { get; set; }
		public List<AttachmentModel>? Attachments { get; set; }
		public string? AttachmentListId { get; set; }
		public string? RevisionIds { get; set; }
		public InvoiceModel? Invoice { get; set; }
		public string? From { get; set; } = null;
		public string? MessageId { get; set; } = null;
		public bool SaveToDatabase { get; set; } = true;
		public bool HasAttachments()
		{
			return Attachments != null && Attachments.Count > 0;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class GetUserInboxSpResult
	{
		public Guid Id { get; set; }
		public Guid? ProjectId { get; set; }
		public string? ProjectName { get; set; }
		public Guid MessageId { get; set; }
		public Guid? ReplyToMessageId { get; set; }
		public string? SenderName { get; set; }
		public string From { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public string? Body { get; set; }
		public DateTime DateCreated { get; set; }
		public bool IsRead { get; set; }
	}
}

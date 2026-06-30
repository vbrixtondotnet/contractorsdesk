using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class EmailProposalPayload
	{
		public Guid ProposalId { get; set; }
		public string To { get; set; }
		public string ReplyTo { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}

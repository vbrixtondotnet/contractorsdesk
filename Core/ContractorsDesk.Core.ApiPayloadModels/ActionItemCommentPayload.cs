using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ActionItemCommentPayload
	{
		public int ActionItemId { get; set; }
		public string Comment { get; set; }
	}
}

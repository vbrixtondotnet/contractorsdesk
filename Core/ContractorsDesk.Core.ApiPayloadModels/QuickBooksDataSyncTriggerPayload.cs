using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class QuickBooksDataSyncTriggerPayload
	{
		public string ContainerId { get; set; } = null!;
		public bool NotifyOnStart { get; set; }
	}
}

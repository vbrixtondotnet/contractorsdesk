using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ActivityStreamPayload
	{
		public List<ActivityStreamItem> Activity { get; set; } = new List<ActivityStreamItem>();
	}
	public class ActivityStreamItem
	{
		public string Id { get; set; } = string.Empty;
		public string Ref { get; set; } = string.Empty;
		public string StepName { get; set; } = string.Empty;
		public string Reason { get; set; } = string.Empty;

	}
}

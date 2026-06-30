using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ProjectJournalPayload
	{
		public string Journal { get; set; } = string.Empty;
		public int CurrentWeek { get; set; } = 1;
	}
}

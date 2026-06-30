using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class MinimumRequestedAmountPayload
	{
		public Guid ProjectId { get; set; }
		public decimal Amount { get; set; }
	}
}

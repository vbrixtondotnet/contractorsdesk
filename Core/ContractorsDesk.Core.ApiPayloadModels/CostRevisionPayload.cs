using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class CostRevisionPayload
	{
		public Guid ProjectId { get; set; }
		public int? ActionItemId { get; set; }

		public Guid EstimateCategoryId { get; set; }

		public decimal Amount { get; set; }

		public decimal CurrentAmount { get; set; }

		public decimal NewAmount { get; set; }
	}
}

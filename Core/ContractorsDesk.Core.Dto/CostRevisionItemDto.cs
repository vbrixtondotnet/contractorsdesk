using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class CostRevisionItemDto
	{
		public Guid Id { get; set; }

		public Guid CostRevisionId { get; set; }

		public Guid EstimateCategoryId { get; set; }

		public decimal Amount { get; set; }

		public decimal CurrentAmount { get; set; }

		public decimal NewAmount { get; set; }
		public string EstimateCategory { get;set; } = string.Empty;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ChangeOrderDto
	{
		public Guid Id { get; set; }

		public int ActionItemId { get; set; }

		public string? CostChangeName { get; set; }

		public decimal? Amount { get; set; }

		public decimal? CurrentAmount { get; set; }

		public decimal? NewAmount { get; set; }

		public string? ScheduleChangeItem { get; set; }

		public int? NoOfDays { get; set; }

		public int? ChangeOrderNumber { get; set; }
	}
}

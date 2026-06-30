using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ActivityStreamDto
	{
		public int Id { get; set; }

		public string RefId { get; set; } = null!;

		public string? StepName { get; set; }

		public string? Reason { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int? UpdatedBy { get; set; }
	}
}

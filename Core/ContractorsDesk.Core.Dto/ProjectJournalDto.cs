using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProjectJournalDto
	{
		public Guid Id { get; set; }

		public Guid ProjectId { get; set; }

		public int? CurrentWeek { get; set; }

		public string Journal { get; set; } = null!;

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int? UpdatedBy { get; set; }
		public bool AppendNewUpdate { get; set; } = true;
		public string ProjectName { get; set; } = null!;
	}
}

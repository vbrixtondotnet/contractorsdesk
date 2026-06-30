using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public partial class ActionItemScheduleChangeDto
	{
		public int Id { get; set; }
		public int? ActionItemId { get; set; }
		public int? NoOfDays { get; set; }
		public int? OriginalNoOfDays { get; set; }
		public Guid? ConstructionTaskId { get; set; }
		public bool? RequiresClientApproval { get; set; }
		public string? ConstructionTaskName { get; set; }


    }
}

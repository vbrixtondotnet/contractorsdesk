using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class WeeklyJobEmailToSupervisorDto
	{
		public string Title { get; set; } = string.Empty;

		public List<string> Descriptions { get; set; } = new List<string>();

		public List<WeeklyJobSupervisorsDto> Supervisors { get; set; } = new List<WeeklyJobSupervisorsDto>();
	}

	public class WeeklyJobSupervisorsDto
	{
		public int UserId { get; set; }

		public string SupervisorsName { get; set; }

		public string SupervisorsEmail { get; set; }
	}
}

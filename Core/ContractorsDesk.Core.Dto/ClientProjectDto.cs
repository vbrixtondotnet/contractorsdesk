using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
    public class ClientProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EstimatedCompletionDate { get; set; }
        public decimal? Budget { get; set; }
        public string Status { get; set; }
        public List<ApplicationUserShortDetailsDto> AssignedSupervisors { get; set; }
    }
}

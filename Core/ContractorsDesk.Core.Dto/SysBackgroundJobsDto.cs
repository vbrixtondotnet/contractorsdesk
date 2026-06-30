using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ContractorsDesk.Core.Dto
{
    public class SysBackgroundJobsDto
    {
        public int SysJobsId { get; set; }
        public string? JobName { get; set; }
        public DateTime? DateOfExecution { get; set; }
        public string? LastStatus { get; set; }
		public string? ApiEndpoint { get; set; }
		public bool? Development { get; set; }
        public bool? Staging { get; set; }
        public bool? Production { get; set; }


    }
}


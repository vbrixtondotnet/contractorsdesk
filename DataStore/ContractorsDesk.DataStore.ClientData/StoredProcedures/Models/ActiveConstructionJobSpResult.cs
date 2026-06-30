using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
    public class ActiveConstructionJobSpResult
    {
        public string ActiveConstructionJobs { get; set; }
        public decimal? JobBalance { get; set; }
        public int? OrderBy { get; set; }
    }
}

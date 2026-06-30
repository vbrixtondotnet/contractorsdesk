using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
    public class ProjectNoteModel
    {
        public Guid? ProjectId { get; set; }

        public string? Notes { get; set; }
    }
}

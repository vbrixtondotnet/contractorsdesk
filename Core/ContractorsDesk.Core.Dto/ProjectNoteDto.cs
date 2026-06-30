using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
    public class ProjectNoteDto
    {
        public Guid Id { get; set; }

        public Guid? ProjectId { get; set; }

        public string? Notes { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public ApplicationUserShortDetailsDto? CreatedByUser { get; set; }
        public ApplicationUserShortDetailsDto? UpdatedByUser { get; set; }
    }
}

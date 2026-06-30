using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
    public class ClientDocumentDto
    {
        public Guid Id { get; set; }

        public Guid FolderId { get; set; }

        public Guid ClientId { get; set; }

        public string? FileName { get; set; }

        public string? FileExtension { get; set; }

        public string Url { get; set; } = null!; 
        public int Version { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public Guid? SubcontractorId { get; set; } 
        public SubContractorDto? Subcontractor { get; set; }
    }
}

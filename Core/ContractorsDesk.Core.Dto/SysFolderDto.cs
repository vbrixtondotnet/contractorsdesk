using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class SysFolderDto
	{
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<ClientDocumentDto> Files { get; set; } = [];
        public List<SysFolderDto> SubFolders { get; set; } = [];
        public int FileCount => Files.Count;

        public Guid? ParentId { get; set; }
        public int Sequence { get; set; }

        //public DateTime DateCreated { get; set; }

        //public int CreatedBy { get; set; }

        //public DateTime? DateUpdated { get; set; }

        //public int? UpdatedBy { get; set; }
    }
}

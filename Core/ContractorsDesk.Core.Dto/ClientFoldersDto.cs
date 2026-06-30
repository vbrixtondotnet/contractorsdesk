using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ClientFoldersDto
	{
		public List<ClientFolder> Folders { get; set; } = [];
	}

	public class ClientFolder
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
        public List<ClientDocumentDto> Files { get; set; } = [];
        public List<ClientFolder> SubFolders { get; set; } = [];
        public int FileCount { get; set; }
		public bool IsSubConractorFolder { get; set; } = false;
    }

	public class ClientFile
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public required string Extension { get; set; }
		public required string Url { get; set; }
		public DateTime DateCreated { get; set; }
	}
}

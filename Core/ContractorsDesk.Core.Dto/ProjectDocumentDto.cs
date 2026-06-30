using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProjectDocumentDto
	{
		public Guid Id { get; set; }

		public Guid ProjectId { get; set; }

		public int DocumentTypeId { get; set; }

		public string FileUrl { get; set; } = null!;

		public DateTime DateAdded { get; set; }

		public bool IsDeleted { get; set; }
	}
}

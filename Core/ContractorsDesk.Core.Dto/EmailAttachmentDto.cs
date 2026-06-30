using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class EmailAttachmentDto
	{
		public int Id { get; set; }

		public Guid EmailId { get; set; }

		public string? FileName { get; set; }

		public string? FileUrl { get; set; }
	}
}

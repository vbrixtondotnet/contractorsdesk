using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class EmailTemplateDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string EmailType { get; set; }

		public string Body { get; set; }

		public int OwnerId { get; set; }

		public bool IsDefault { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedById { get; set; }

		public DateTime DateModified { get; set; }

		public int ModifiedById { get; set; }
	}
}

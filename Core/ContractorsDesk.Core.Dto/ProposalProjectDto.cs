using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalProjectDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		public string? Address { get; set; }

		public string? City { get; set; }

		public string? State { get; set; }

		public string? Description { get; set; }

		public int? SqFeet { get; set; }

		public DateTime DateCreated { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int? UpdatedBy { get; set; }
	}
}

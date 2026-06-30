using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class InvoiceItemDto
	{
		public Guid Id { get; set; }

		public Guid InvoiceId { get; set; }

		public string Description { get; set; } = null!;

		public int Quantity { get; set; }

		public decimal Rate { get; set; }

		public decimal Amount { get; set; }

		public int Sequence { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }
	}
}

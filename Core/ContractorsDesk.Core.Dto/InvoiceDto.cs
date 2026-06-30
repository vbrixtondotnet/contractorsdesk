using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class InvoiceDto
	{
		public Guid Id { get; set; }

		public string InvoiceNumber { get; set; } = null!;

		public Guid ClientId { get; set; }

		public DateTime InvoiceDate { get; set; }

		public DateTime DueDate { get; set; }

		public decimal TotalAmount { get; set; }

		public string Status { get; set; } = null!;

		public DateTime DateCreated { get; set; }

		public DateTime? DateUpdated { get; set; }

		public int CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }
		public string BillTo { get; set; }
		public List<InvoiceItemDto> InvoiceItems { get; set; } = new List<InvoiceItemDto>();
	}
}

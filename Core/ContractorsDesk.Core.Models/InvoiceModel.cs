using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class InvoiceModel
	{
		public Guid Id { get; set; }

		public string InvoiceNumber { get; set; } = null!;

		public Guid ClientId { get; set; }

		public DateTime InvoiceDate { get; set; }

		public DateTime DueDate { get; set; }

		public decimal TotalAmount
		{
			get
			{
				return Items?.Sum(item => item.Amount) ?? 0;
			}
		}

		public string Status { get; set; } = null!;

		public List<InvoiceItemModel> Items { get; set; }
	}

	public class InvoiceItemModel
	{

		public string Description { get; set; } = null!;

		public int Quantity { get; set; }

		public decimal Rate { get; set; }

		public decimal Amount { get; set; }
	}
}

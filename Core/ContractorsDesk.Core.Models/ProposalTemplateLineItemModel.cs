using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class ProposalTemplateLineItemModel
	{
		public Guid Id { get; set; }
		public Guid ProposalTemplateId { get; set; }
		public Guid? EstimateCategoryId{ get; set; }
		public string Name{ get; set; }
		public string? Description { get; set; }
		public decimal? Amount { get; set; }
		public int Sequence { get; set; }
		public Guid? ParentId { get; set; }
		public List<ProposalTemplateLineItemModel>? LineItems { get; set; } = [];
		public decimal? Percentage{ get; set; }
		public decimal? SqFoot { get; set; }
		public bool SqFootLocked { get; set; }
		public decimal? Multiplier { get; set; }
		public decimal TotalAmount
		{
			get
			{
				if (this.LineItems != null && this.LineItems.Any())
				{
					return this.LineItems.Select(x => x.Amount ?? 0).Sum();
				}
				return 0;
			}
		}
	}
}

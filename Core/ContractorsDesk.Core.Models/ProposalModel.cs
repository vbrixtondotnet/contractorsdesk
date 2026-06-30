using ContractorsDesk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class ProposalModel
	{
		public Guid Id { get; set; }
		public Guid? QbClassId { get; set; }
		public int Number { get; set; }
		public int Status { get; set; }
		public string Date { get; set; }
		public Guid? MergeWithProjectId { get; set; } = null;
		public Guid? OverwriteProposalId { get; set; } = null;
		public List<int>? Supervisors { get; set; } = new List<int>();
		public ClientModel? Client { get; set; } = new ClientModel();
		public ProposalProjectModel? Project { get; set; } = new ProposalProjectModel();
		public ProposalTemplateModel Template { get; set; } = new ProposalTemplateModel();

		public decimal TotalAmount
		{
			get
			{
				decimal retval = 0;
				if (this.Template.Categories != null && this.Template.Categories.Any())
				{
					var lineItems = this.Template.Categories.SelectMany(c => c.LineItems);
					if (lineItems != null)
					{
						retval = (decimal)lineItems.Sum(p => p.Amount ?? 0);
					}
				}
				return retval;
			}
		}
	}
}

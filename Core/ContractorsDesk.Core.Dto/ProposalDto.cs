using ContractorsDesk.Core.Enums;

namespace ContractorsDesk.Core.Dto
{
	public class ProposalDto
	{
		public Guid Id { get; set; }
		public Guid? QbClassId { get; set; }
		public int Number { get; set; }
		public int Status { get; set; }
		public string Date { get; set; }
		public List<int> Supervisors { get; set; } = new List<int>();
		public ClientDto? Client { get; set; } = new ClientDto();
		public ProposalProjectDto? Project { get; set; } = new ProposalProjectDto();
		public ProposalTemplateDto Template { get; set; } = new ProposalTemplateDto();

		public decimal Total
		{
			get
			{
				decimal retval = 0;
				if (this.Template != null && this.Template.Categories != null && this.Template.Categories.Any())
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
		public decimal TotalGCFeePercentage
		{
			get
			{
				decimal retval = 0;
				if (this.Template != null && this.Template.Categories != null && this.Template.Categories.Any())
				{
					var gcFeeItems = new List<string> {
						"On Site Supervision",
						"GC fee",
						"General Contractor Fee"
					};

					var lineItems = this.Template.Categories.SelectMany(c => c.LineItems).Where(li=> gcFeeItems.Contains(li.Name));
					if (lineItems != null)
					{
						retval = (decimal)lineItems.Sum(p => p.Percentage ?? 0);
					}
				}
				return retval;
			}
		}
		public decimal TotalGCFeeAmount
		{
			get
			{
				decimal retval = 0;
				if (this.Template != null && this.Template.Categories != null && this.Template.Categories.Any())
				{
					var onsiteSupervisionItems = new List<string> {
						"On Site Supervision",
						"GC fee",
						"General Contractor Fee"
					};

					var lineItems = this.Template.Categories.SelectMany(c => c.LineItems).Where(li => onsiteSupervisionItems.Contains(li.Name));
					if (lineItems != null)
					{
						retval = (decimal)lineItems.Sum(p => p.Amount ?? 0);
					}
				}
				return retval;
			}
		}
		public string StatusString
		{
			get
			{
				return ((DocStatus)this.Status).GetStringValue();
			}
		}
		public bool Locked { get; set; }
		public decimal TotalAmount()
		{
			decimal retval = 0;
			if (this.Template.Categories != null && this.Template.Categories.Any())
			{
				var lineItems = this.Template.Categories.SelectMany(c => c.LineItems);
				if(lineItems != null)
				{
					retval = (decimal)lineItems.Sum(p => p.Amount ?? 0);
				}
			}
			return retval;
		}
		public decimal GetTotalNonOverheadAmount()
		{
			decimal retval = 0;
			if (this.Template.Categories != null && this.Template.Categories.Any())
			{
				var lineItems = this.Template.Categories.Where(c => c.Name.ToLower() != "overhead").SelectMany(c => c.LineItems).ToList();
				if (lineItems != null)
				{
					retval = (decimal)lineItems.Sum(p => p.Amount ?? 0);
				}
			}
			return retval;
		}
		public bool? HasProjectSchedule { get; set; }
		public bool? IncludeLinesWithZeroAmount { get; set; }
    }
}

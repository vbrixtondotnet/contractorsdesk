namespace ContractorsDesk.Core.Dto
{
	public class ProposalTemplateLineItemDto
	{
		public Guid Id { get; set; }
		public Guid ProposalTemplateId { get; set; }

		private Guid? _estimateCategoryId;
		public Guid? EstimateCategoryId {
			get => _estimateCategoryId ?? Guid.Empty;
			set => _estimateCategoryId = value;
		}

		private string _name;
		public string Name
		{
			get => _name ?? string.Empty; // Returns string.Empty if _description is null
			set => _name = value;
		}

		private string _description;
		public string Description
		{
			get => _description ?? string.Empty; // Returns string.Empty if _description is null
			set => _description = value;
		}
		public decimal? Amount { get; set; }
		public int Sequence { get; set; }

		public Guid? ParentId { get; set; }

		public List<ProposalTemplateLineItemDto>? LineItems { get; set; } = [];

		public decimal TotalAmount
		{
			get
			{
				if(this.LineItems != null && this.LineItems.Any())
				{
					return this.LineItems.Select(x => x.Amount ?? 0).Sum();
				}
				return 0;
			}
		}

		private decimal? _percentage;
		public decimal? Percentage { 
			get => _percentage ?? (this.Name.ToUpper() == "ON SITE SUPERVISION" ? 5 : (this.Name.ToUpper() == "GENERAL CONTRACTOR FEE" ? 10 : 0));
			set => _percentage = value;
		}
		public decimal? SqFoot { get; set; }
		public decimal? Multiplier { get; set; }
		public bool SqFootLocked { get; set; }
	}
}

namespace ContractorsDesk.Core.Dto
{
	public class EstimateToActualDto
	{
		public ProjectDetailsDto ProjectDetails { get; set; }
		public List<RevisedEstimateCategoryDto> EstimateCategories { get; set; }
		public Summary Summary { get; set; }
		public List<UmmappedTransactions> UnmappedTransactions { get; set; }
		public bool Locked { get; set; }
	}
	public class RevisedEstimateCategoryDto
	{
		public Guid Id { get; set; }

		private string _name;
		public string Name
		{
			get => _name ?? string.Empty;
			set => _name = value;
		}
		public int? Sequence { get; set; }
		public List<RevisedEstimateCategoryLineDto> LineItems { get; set; } = new List<RevisedEstimateCategoryLineDto>();
		public decimal TotalOriginal => GetTotals(x => x.Original);
		public decimal TotalRevised => GetTotals(x => x.Revised);
		public decimal TotalCostToDate => GetTotals(x => x.CostToDate);
		public decimal TotalBalance => GetTotals(x => x.Balance);
		public decimal TotalPercentage => GetTotalPercentage();
		private decimal GetTotals(Func<RevisedEstimateCategoryLineDto, decimal?> selector)
		{
			return this.LineItems?.Select(x => selector(x) ?? 0).Sum() ?? 0;
		}

		private decimal GetTotalPercentage()
		{
			if (this.LineItems.Count > 0)
			{
				return Decimal.Round(((this.LineItems?.Select(x => x.Percentage ?? 0).Sum() ?? 0) / this.LineItems.Count));
			}
			return 0;
		}

	}

	public class RevisedEstimateCategoryLineDto
	{
		public Guid? Id { get; set; }
		public Guid? ProposalLineId { get; set; }
		public Guid? EstimateCategoryId { get; set; }
		public Guid? ParentId { get; set; }

		private string _name;
		public string Name
		{
			get => _name ?? string.Empty;
			set => _name = value;
		}
		public int? Sequence { get; set; }
		public decimal? Original { get; set; }

		private decimal? _revised;
		public decimal? Revised { get { return this._revised ?? 0; } set { this._revised = value; } }
		public decimal? CurrentRevisedValue { get; set; }

		private decimal? costToDate;
        public decimal? CostToDate
		{
			get
			{
				return costToDate ?? 0;
            }
			set
			{
				this.costToDate = value;
			}
		}

		public decimal? balance;
		public decimal? Balance
		{
			get
			{
				return this.Revised - this.CostToDate;
			}
			set
			{
				this.balance = value;
			}
		}

		public decimal? percentage;
		public decimal? Percentage
		{
			get
			{
                if (this.Revised == this.CostToDate)
                {
                    return 100;
                }
                else if (this.Revised == 0)
                {
                    return null;
                }
                else
                {
                    var percentage = (decimal)((this.CostToDate / this.Revised) * 100);
                    return Math.Round(percentage);
                }
            }
			set
			{
				this.percentage = value;
			}
		}

		public bool HasEstimateMapping { get; set; } = true;

	}

	public class Summary
	{
		public decimal? TotalCostToDate { get; set; }
		public decimal? OwnerDeposits { get; set; }
		public decimal? JobBalance { get; set; }
		public decimal? MinimumRequestedAmount { get; set; }
	}

	public class UmmappedTransactions
	{
		public Guid AccountId { get; set; }
		public string Name { get; set; }
		public decimal? CostToDate { get; set; }
	}
}

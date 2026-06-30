namespace ContractorsDesk.Core.Dto
{
	public class TransactionDetailsDto
	{
		public string Category {  get; set; }
		public int Sequence {  get; set; }
		public List<TransactionDetailsLineItemDto> LineItems { get; set; }
		public decimal? TotalAmount
		{
			get
			{
				return this.LineItems.Sum(m => m.Total);
			}
		}
	}
	public class TransactionDetailsLineItemDto
	{
		public string Name {  get; set; }
		public int Sequence { get; set; }

		private decimal? _revisedEstimate;
		public decimal? RevisedEstimate { get { return _revisedEstimate ?? 0; } set{ this._revisedEstimate = value; } }
		public decimal? Balance
		{
			get
			{
				return RevisedEstimate - Total;
			}
		}
		public decimal? Total
		{
			get
			{
				return this.Transactions.Sum(t => t.Amount);
			}
		}
		public List<TransactionsDto> Transactions { get; set; }

	}
	public class TransactionsDto
	{
		public DateTime Date { get; set; }
		public string Type { get; set; }
		public string Num { get; set; }
		public string Payee { get; set; }
		public string Memo { get; set; }
		public decimal Amount { get; set; }

		public string FormattedDate
		{
			get
			{
				//DateTime parsedDate = DateTime.Parse(this.Date);
				return this.Date.ToString("MM/dd/yyyy");
			}
		}

	}
}

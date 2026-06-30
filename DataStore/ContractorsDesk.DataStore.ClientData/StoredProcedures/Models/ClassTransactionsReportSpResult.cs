using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class ClassTransactionsReportSpResult
	{
		private string date;
		private string? type;
		private string? clearStatus;
		private string? num;
		private string? name;
		private string? division;
		private string? cls;
		private string? memo;
		private string? sourceAccount;
		private string? category;
		private decimal? amount;
		private decimal? balance;
		public string Date { get { return this.date ?? string.Empty; } set { this.date = value; } }
		public string? Type { get { return this.type ?? string.Empty; } set { this.type = value; } }
		public string? ClearStatus { get { return this.clearStatus ?? string.Empty; } set { this.clearStatus = value; } }
		public string? Num { get { return this.num ?? string.Empty; } set { this.num = value; } }
		public string? Name { get { return this.name ?? string.Empty; } set { this.name = value; } }
		public string? Division { get { return this.division ?? string.Empty; } set { this.division = value; } }
		public string Class { get { return this.cls ?? string.Empty; } set { this.cls = value; } }
		public string? Memo { get { return this.memo ?? string.Empty; } set { this.memo = value; } }
		public string? SourceAccount { get { return this.sourceAccount ?? string.Empty; } set { this.sourceAccount = value; } }
		public string? Category { get { return this.category ?? string.Empty; } set { this.category = value; } }
		public decimal? Amount { get { return this.amount ?? 0; } set { this.amount = value; } }
		public decimal? Balance { get { return this.balance ?? 0; } set { this.balance = value; } }
	}
}

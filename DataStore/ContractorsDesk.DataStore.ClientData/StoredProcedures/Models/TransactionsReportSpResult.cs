using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class TransactionsReportSpResult
	{
		string? date;
		string? type;
		string? num;
		string? clearStatus;
		decimal? amount;
		decimal? balance;
		string? _class;
		string? division;
		string? category;
		string? memo;
		string? customerPayee;
		public string? Date { get { return this.date ?? string.Empty; } set { this.date = value; } }
		public string? Type { get { return this.type ?? string.Empty; } set { this.type = value; } }
		public string? Num { get { return this.num ?? string.Empty; } set { this.num = value; } }
		public string? ClearStatus { get { return this.clearStatus ?? string.Empty; } set { this.clearStatus = value; } }
		public decimal? Amount { get { return this.amount ?? 0; } set { this.amount = value; } }
		public decimal? Balance { get { return this.balance ?? 0; } set { this.balance = value; } }
		public string? Class { get { return this._class ?? string.Empty; } set { this._class = value; } }
		public string? Division { get { return this.division ?? string.Empty; } set { this.division = value; } }
		public string? Category { get { return this.category ?? string.Empty; } set { this.category = value; } }
		public string? Memo { get { return this.memo ?? string.Empty; } set { this.memo = value; } }
		public string? CustomerPayee { get { return this.customerPayee ?? string.Empty; } set { this.customerPayee = value; } }
	}
}

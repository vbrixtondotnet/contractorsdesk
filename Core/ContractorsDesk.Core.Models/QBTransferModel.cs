using ContractorsDesk.Core.Models.QBSubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class QBTransferModel : QBTransactionEntity<QBTransferLineModel>, IQBBaseEntity
	{
		public QBAccountReferenceModel? FromAccountRef { get; set; }
		public QBAccountReferenceModel? ToAccountRef { get; set; }
		public decimal Amount { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public string TxnDate { get; set; }
		public QBCurrencyModel CurrencyRef { get; set; }
		public string PrivateNote { get; set; }
	}

	public class QBTransferLineModel : IQBBaseLineEntity
	{ }
}

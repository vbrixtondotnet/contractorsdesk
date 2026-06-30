using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBDepositModel : QBTransactionEntity<QBDepositLineModel>, IQBBaseEntity
	{
		public QBAccountReferenceModel DepositToAccountRef { get; set; }
		public decimal TotalAmt { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public string DocNumber { get; set; }
		public string TxnDate { get; set; }
		public QBDepartmentReferenceModel DepartmentRef { get; set; }
		public QBCurrencyModel CurrencyRef { get; set; }
	}

	public class QBDepositLineModel : IQBBaseLineEntity
	{
		public string Id { get; set; }
		public int LineNum { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string DetailType { get; set; }
		public QBDepositLineDetail DepositLineDetail { get; set; }
	}

	public class QBDepositLineDetail
	{
		public QBEntityReferenceModel Entity { get; set; }
		public QBClassReferenceModel ClassRef { get; set; }
		public QBAccountReferenceModel AccountRef { get; set; }
		public QBPaymentMethodReferenceModel PaymentMethodRef { get; set; }
	}
}

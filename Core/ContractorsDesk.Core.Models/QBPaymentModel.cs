using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBPaymentModel : QBTransactionEntity<QBPaymentLineModel>, IQBBaseEntity
	{
		public QBCustomerReferenceModel CustomerRef { get; set; }
		public QBDepositToAccountReferenceModel DepositToAccountRef { get; set; }
		public QBPaymentMethodReferenceModel PaymentMethodRef { get; set; }
		public string PaymentRefNum { get; set; }
		public decimal TotalAmt { get; set; }
		public decimal UnappliedAmt { get; set; }
		public bool ProcessPayment { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public string TxnDate { get; set; }
		public QBCurrencyModel CurrencyRef { get; set; }
		public string PrivateNote { get; set; }
		public List<QBLinkedTxnModel> LinkedTxn { get; set; }
	}

	public class QBPaymentLineModel : IQBBaseLineEntity
	{
		public decimal Amount { get; set; }
		public List<QBLinkedTxnModel> LinkedTxn { get; set; }
	}
}

using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBBillPaymentModel : QBTransactionEntity<QBBillPaymentPaymentLineModel>, IQBBaseEntity
	{
		public QBVendorReferenceModel VendorRef { get; set; }
		public string PayType { get; set; }
		public QBCheckPaymentModel CheckPayment { get; set; }
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

	public class QBBillPaymentPaymentLineModel : IQBBaseLineEntity
	{
		public decimal Amount { get; set; }
		public List<QBLinkedTxnModel> LinkedTxn { get; set; }
	}
}

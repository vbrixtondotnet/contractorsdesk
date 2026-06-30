using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBSalesReceiptModel : QBTransactionEntity<QBSalesReceiptLineModel>, IQBBaseEntity
	{
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public List<object> CustomField { get; set; } // Could be extended if needed
		public string DocNumber { get; set; }
		public string TxnDate { get; set; }
		public QBDepartmentReferenceModel DepartmentRef { get; set; }
		public QBCurrencyModel CurrencyRef { get; set; }
		public string PrivateNote { get; set; }
		public QBCustomerReferenceModel CustomerRef { get; set; }
		public decimal TotalAmt { get; set; }
		public bool ApplyTaxAfterDiscount { get; set; }
		public string PrintStatus { get; set; }
		public string EmailStatus { get; set; }
		public decimal Balance { get; set; }
		public QBDepositToAccountReferenceModel DepositToAccountRef { get; set; }
	}

	public class QBSalesReceiptLineModel : IQBBaseLineEntity
	{
		public string Id { get; set; }
		public int? LineNum { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string DetailType { get; set; }

		public QBSalesItemLineDetailModel SalesItemLineDetail { get; set; }
		public QBSubTotalLineDetailModel SubTotalLineDetail { get; set; }
	}
}

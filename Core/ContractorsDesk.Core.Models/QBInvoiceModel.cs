using ContractorsDesk.Core.Models.QBSubModels;
namespace ContractorsDesk.Core.Models
{
	public class QBInvoiceModel : QBTransactionEntity<QBInvoiceLineModel>, IQBBaseEntity
	{
		public bool AllowIPNPayment { get; set; }
		public bool AllowOnlinePayment { get; set; }
		public bool AllowOnlineCreditCardPayment { get; set; }
		public bool AllowOnlineACHPayment { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public List<QBCustomFieldModel> CustomField { get; set; }
		public string DocNumber { get; set; }
		public string TxnDate { get; set; }
		public QBDepartmentReferenceModel DepartmentRef { get; set; }
		public QBCurrencyModel CurrencyRef { get; set; }
		public List<QBLinkedTxnModel> LinkedTxn { get; set; }
		public QBCustomerReferenceModel CustomerRef { get; set; }
		public QBBillAddressModel BillAddr { get; set; }
		public QBShipAddressModel ShipAddr { get; set; }
		public QBClassReferenceModel ClassRef { get; set; }
		public QBSalesTermReferenceModel SalesTermRef { get; set; }
		public string DueDate { get; set; }
		public decimal TotalAmt { get; set; }
		public bool ApplyTaxAfterDiscount { get; set; }
		public string PrintStatus { get; set; }
		public string EmailStatus { get; set; }
		public decimal Balance { get; set; }
	}

	public class QBInvoiceLineModel : IQBBaseLineEntity
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

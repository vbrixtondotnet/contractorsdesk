using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBBillModel : QBTransactionEntity<QBBillLineModel>, IQBBaseEntity
	{
		public QBSalesTermReferenceModel? SalesTermRef { get; set; }
		public string DueDate { get; set; }
		public decimal Balance { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string DocNumber { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel? MetaData { get; set; }
		public string TxnDate { get; set; }
		public QBDepartmentReferenceModel? DepartmentRef { get; set; }
		public QBCurrencyModel? CurrencyRef { get; set; }
		public List<QBLinkedTxnModel> LinkedTxn { get; set; }
		//public List<QBBillLineModel> Line { get; set; }
		public QBVendorReferenceModel? VendorRef { get; set; }
		public QBAccountReferenceModel? APAccountRef { get; set; }
		public decimal TotalAmt { get; set; }
	}

	public class QBBillLineModel : IQBBaseLineEntity
	{
		public string Id { get; set; }
		public int LineNum { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string DetailType { get; set; }
		public QBAccountBasedExpenseLineDetailModel? AccountBasedExpenseLineDetail { get; set; }
	}
}

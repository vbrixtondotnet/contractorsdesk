using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBPurchaseModel : QBTransactionEntity<QBPurchaseLineModel>, IQBBaseEntity
	{
		public QBAccountReferenceModel AccountRef { get; set; }
		public string PaymentType { get; set; }
		public QBEntityReferenceModel EntityRef { get; set; }
		public QBRemitToAddressModel RemitToAddr { get; set; }
		public decimal TotalAmt { get; set; }
		public string PrintStatus { get; set; }
		public QBPurchaseExModel PurchaseEx { get; set; }
		public string Domain { get; set; }
		public string Status { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public string DocNumber { get; set; }
		public string TxnDate { get; set; }
        public QBDepartmentReferenceModel DepartmentRef { get; set; }
        public QBCurrencyModel CurrencyRef { get; set; }
		public string PrivateNote { get; set; }
	}

	public class QBPurchaseExModel
	{
		public List<QBPurchaseNameValueWrapperModel> Any { get; set; }
	}

	public class QBPurchaseNameValueWrapperModel
	{
		public string Name { get; set; }
		public string DeclaredType { get; set; }
		public string Scope { get; set; }
		public QBPurchaseNameValueModel Value { get; set; }
		public bool Nil { get; set; }
		public bool GlobalScope { get; set; }
		public bool TypeSubstituted { get; set; }
	}

	public class QBPurchaseNameValueModel
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

	public class QBPurchaseLineModel : IQBBaseLineEntity
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string DetailType { get; set; }
		public QBAccountBasedExpenseLineDetailModel AccountBasedExpenseLineDetail { get; set; }
	}
}

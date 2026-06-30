using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBJournalEntryModel : QBTransactionEntity<QBJournalLineModel>, IQBBaseEntity
	{
		public bool Adjustment { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
		public string DocNumber { get; set; }
		public string TxnDate { get; set; }
		public QBCustomerReferenceModel CurrencyRef { get; set; }
		public List<QBJournalLineModel> Line { get; set; }
	}

	public class QBJournalLineModel : IQBBaseLineEntity
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string DetailType { get; set; }
		public QBJournalEntryLineDetailModel JournalEntryLineDetail { get; set; }
	}

	public class QBJournalEntryLineDetailModel
	{
		public string PostingType { get; set; }
		public QBEntityDetailModel Entity { get; set; }
		public QBAccountReferenceModel AccountRef { get; set; }
		public QBClassReferenceModel ClassRef { get; set; }
		public QBDepartmentReferenceModel DepartmentRef { get; set; }
	}
}

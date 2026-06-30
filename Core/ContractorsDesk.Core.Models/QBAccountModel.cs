using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBAccountModel : IQBBaseEntity
	{
		public string Name { get; set; }
		public bool SubAccount { get; set; }
		public QBParentReferenceModel? ParentRef { get; set; }
		public string FullyQualifiedName { get; set; }
		public bool Active { get; set; }
		public string Classification { get; set; }
		public string AccountType { get; set; }
		public string AccountSubType { get; set; }
		public string AcctNum { get; set; }
		public double CurrentBalance { get; set; }
		public double CurrentBalanceWithSubAccounts { get; set; }
		public QBCurrencyModel? CurrencyRef { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel? MetaData { get; set; }
	}
}

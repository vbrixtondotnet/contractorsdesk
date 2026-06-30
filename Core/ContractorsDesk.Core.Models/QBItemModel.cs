using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
	public class QBItemModel : IQBBaseEntity
	{
		public string Name { get; set; }
		public bool Active { get; set; }
		public string FullyQualifiedName { get; set; }
		public bool Taxable { get; set; }
		public decimal UnitPrice { get; set; }
		public string Type { get; set; }
		public QBIncomeAccountReferenceModel IncomeAccountRef { get; set; }
		public decimal PurchaseCost { get; set; }
		public QBAccountReferenceModel ExpenseAccountRef { get; set; }
		public bool TrackQtyOnHand { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel MetaData { get; set; }
	}
}

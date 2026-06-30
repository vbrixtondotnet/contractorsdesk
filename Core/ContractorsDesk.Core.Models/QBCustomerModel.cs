using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBCustomerModel : IQBBaseEntity
	{
		public string Id { get; set; }
		public string GivenName { get; set; }
		public string FamilyName { get; set; }
		public string FullyQualifiedName { get; set; }
		public string CompanyName { get; set; }
		public QBBillAddressModel? BillAddr { get; set; }
		public decimal? Balance { get; set; }
		public QBCurrencyModel? CurrencyRef { get; set; }
		public QBPrimaryPhoneModel? PrimaryPhone { get; set; }
		public QBPrimaryEmailAddressModel? PrimaryEmailAddr { get; set; }
        public bool Active { get; set; }
        public int Level { get; set; }
		public QBParentReferenceModel? ParentRef { get; set; }
		public QBMetaDataModel? MetaData { get; set; }
	}
}

using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBVendorModel : IQBBaseEntity
	{
        public QBBillAddressModel? BillAddr { get; set; }
        public double Balance { get; set; }
        public bool Vendor1099 { get; set; }
		public QBCurrencyModel? CurrencyRef { get; set; }
		public string Domain { get; set; }
		public bool Sparse { get; set; }
		public string Id { get; set; }
		public string SyncToken { get; set; }
		public QBMetaDataModel? MetaData { get; set; }
		public string GivenName { get; set; }
		public string MiddleName { get; set; }
		public string FamilyName { get; set; }
		public string CompanyName { get; set; }
		public string DisplayName { get; set; }
		public string PrintOnCheckName { get; set; }
		public bool Active { get; set; }
        public QBPrimaryPhoneModel? PrimaryPhone { get; set; }
        public QBAlternatePhoneModel? AlternatePhone { get; set; }
        public QBPrimaryEmailAddressModel? PrimaryEmailAddr { get; set; }
    }
}

namespace ContractorsDesk.Core.Models.QBSubModels
{
	public class QBAccountBasedExpenseLineDetailModel
	{
		public QBCustomerReferenceModel? CustomerRef { get; set; }
		public QBClassReferenceModel? ClassRef { get; set; }
		public QBAccountReferenceModel? AccountRef { get; set; }
		public string BillableStatus { get; set; }
		public QBTaxCodeReferenceModel? TaxCodeRef { get; set; }
	}
}

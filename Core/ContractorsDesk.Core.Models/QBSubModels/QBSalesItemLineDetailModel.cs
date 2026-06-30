namespace ContractorsDesk.Core.Models.QBSubModels
{
	public class QBSalesItemLineDetailModel
	{
		public string ServiceDate { get; set; }
		public QBItemReferenceModel ItemRef { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Qty { get; set; }
		public QBTaxCodeReferenceModel TaxCodeRef { get; set; }
	}
}

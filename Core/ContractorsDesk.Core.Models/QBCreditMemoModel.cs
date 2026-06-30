using ContractorsDesk.Core.Models.QBSubModels;

namespace ContractorsDesk.Core.Models
{
    public class QBCreditMemoModel : QBTransactionEntity<QBCreditMemoLineModel>, IQBBaseEntity
	{
        public decimal RemainingCredit { get; set; }
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
        public QBCustomerReferenceModel CustomerRef { get; set; }
        public QBClassReferenceModel ClassRef { get; set; }
        public decimal TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        public QBPrimaryEmailAddressModel BillEmail { get; set; }
        public decimal Balance { get; set; }
        public QBDeliveryInfoModel DeliveryInfo { get; set; }
    }

    public class QBCreditMemoLineModel : IQBBaseLineEntity
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

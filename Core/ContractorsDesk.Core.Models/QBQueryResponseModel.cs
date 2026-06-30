using Newtonsoft.Json;

namespace ContractorsDesk.Core.Models
{
	public class QBQueryResponseModel<T>
	{
		[JsonProperty("Account")]
		public List<T> QBAccountData { get; set; }

		[JsonProperty("Vendor")]
		public List<T> QBVendorData { get; set; }

		[JsonProperty("Class")]
		public List<T> QBClassData { get; set; }

		[JsonProperty("Customer")]
		public List<T> QBCustomerData { get; set; }

		[JsonProperty("Item")]
		public List<T> QBItemData { get; set; }

		[JsonProperty("Bill")]
		public List<T> QBBillData { get; set; }

		[JsonProperty("BillPayment")]
		public List<T> QBBillPaymentData { get; set; }

		[JsonProperty("CreditMemo")]
		public List<T> QBCreditMemoData { get; set; }

		[JsonProperty("Deposit")]
		public List<T> QBDepositData { get; set; }

		[JsonProperty("Invoice")]
		public List<T> QBInvoiceData { get; set; }

		[JsonProperty("Purchase")]
		public List<T> QBPurchaseData { get; set; }

		[JsonProperty("JournalEntry")]
		public List<T> QBJournalEntryData { get; set; }

		[JsonProperty("Payment")]
		public List<T> QBPaymentData { get; set; }

		[JsonProperty("SalesReceipt")]
		public List<T> QBSalesReceiptData { get; set; }

		[JsonProperty("Transfer")]
		public List<T> QBTransferData { get; set; }

		public List<T> GetTransactionData(string transactionType)
		{
			return transactionType switch
			{
				"Account" => QBAccountData,
				"Vendor" => QBVendorData,
				"Class" => QBClassData,
				"Customer" => QBCustomerData,
				"Item" => QBItemData,
				"Bill" => QBBillData,
				"BillPayment" => QBBillPaymentData,
				"CreditMemo" => QBCreditMemoData,
				"Deposit" => QBDepositData,
				"Invoice" => QBInvoiceData,
				"Purchase" => QBPurchaseData,
				"JournalEntry" => QBJournalEntryData,
				"Payment" => QBPaymentData,
				"SalesReceipt" => QBSalesReceiptData,
				"Transfer" => QBTransferData,
				_ => new List<T>()
			};
		}
	}
}

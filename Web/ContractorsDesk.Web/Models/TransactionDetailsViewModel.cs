using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;

namespace ContractorsDesk.WebPortal.Models
{
	public class TransactionDetailsViewModel
	{
		public string? TransactionDate { get; set; }
		public Guid ProposalId { get; set; }
		public List<TransactionDetailsDto> TransactionDetails { get; set; }
		public decimal TotalCost { get; set; }
		public string ProjectName {  get; set; }

		private string itemName;
		public string ItemName
		{
			get
			{
				return IsOwnerDepositsReport ? "Owner Deposits" : itemName;
			}
			set { this.itemName = value; }
		}

		public bool IsOwnerDepositsReport { get; set; } = false;
	}
}

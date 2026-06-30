using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ClientContractDetails
	{
		public string ClientName { get; set; }
		public string ClientEmail { get; set; }
		public string ClientAddress { get; set; }
		public string ClientCity { get; set; }
		public string ClientState { get; set; }
		public string EstStartDate { get; set; }
		public string EstCompletionDate { get; set; }
		public decimal GenContractorsFeePercentage { get; set; }
		public decimal GenContractorsFeeAmount { get; set; }
		public decimal ProjectCost { get; set; }
		public int InitialDepositPercentage { get; set; }
		public string Contractor { get; set; }
		public string ContractorEmailAddress { get; set; }

	}
}

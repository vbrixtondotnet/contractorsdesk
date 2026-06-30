using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContractorsDesk.Core.Dto
{
	public class JobsSummaryReportDto
	{
		public string JobAddress { get; set; }
		private string _startDate { get; set; }
		private decimal? _estContractPrice { get; set; }
		private string? _onSiteSupervisor { get; set; }
		private decimal? _pendingOverhead { get; set; }
		private decimal? _balanceOnsite { get; set; }
		private decimal? _balanceProject { get; set; }
		public string StartDate {
			get => _startDate ?? string.Empty;
			set => _startDate = value;
		}
		public decimal? EstContractPrice {
			get => _estContractPrice ?? 0;
			set => _estContractPrice = value;
		}
		public string OnSiteSupervisor
		{
			get => _onSiteSupervisor ?? string.Empty;
			set => _onSiteSupervisor = value;
		}
		public decimal? PendingOverhead
		{
			get => _pendingOverhead ?? 0;
			set => _pendingOverhead = value;
		}
		public decimal? BalanceOnsite
		{
			get => _balanceOnsite ?? 0;
			set => _balanceOnsite = value;
		}
		public decimal? BalanceProject
		{
			get => _balanceProject ?? 0;
			set => _balanceProject = value;
		}
	}
}

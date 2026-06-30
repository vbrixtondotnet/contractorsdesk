using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Dto
{
	public class DashboardStatsDto
	{
		public ProjectStats ProjectStats { get; set; }
		public ProjectBalances ProjectBalances { get; set; }
		public List<RecentClient> RecentClients { get; set; }
		public int TotalClients { get; set; }
	}

	public class ProjectStats
	{
		public int ActiveCount { get; set; }
		public int CompletedCount { get; set; }
		public int ArchivedCount { get; set; }
		public int TotalCount => ActiveCount + CompletedCount + ArchivedCount;
	}

	public class ProjectBalances
	{
		public int AboveThresholdCount { get; set; }
		public int BelowThresholdCount { get; set; }
		public int NegativeBalanceCount { get; set; }
		public int TotalCount => AboveThresholdCount + BelowThresholdCount + NegativeBalanceCount;

	}

	public class RecentClient
	{
		public string Name { get; set; }

		public string Initials
		{
			get {
				return Name.Length > 1 ? Name[0].ToString() + Name[1].ToString() : Name[0].ToString();
			}
		}

	}
}

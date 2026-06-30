using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class ProjectEstimateCategoriesSpResult
	{
		public Guid EstimateCategoryID { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal? CurrentAmount { get; set; }
	}
}

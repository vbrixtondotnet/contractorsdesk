using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.Interfaces
{
	public interface IAuditableEntity
	{
		int CreatedBy { get; set; }
		int? UpdatedBy { get; set; }
		DateTime DateCreated { get; set; }
		DateTime? DateUpdated { get; set; }
	}
}

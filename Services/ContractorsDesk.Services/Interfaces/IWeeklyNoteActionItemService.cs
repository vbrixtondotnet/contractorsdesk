using ContractorsDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IWeeklyNoteActionItemService
	{
		Task CreateWeeklyNoteActionItem(SignalRMessageModel? signalRMessageModel = null);
	}
}

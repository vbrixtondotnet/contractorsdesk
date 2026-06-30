using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class NotificationResponseModel
	{
		public bool Ok { get; set; }
		public int ErrorCode { get; set; }
		public string Description { get; set; }
		public TelegramMessageModel? TelegramMessage { get; set; } = null;
		public SignalRMessageModel? SignalRMessageModel { get; set; } = null;
	}
}

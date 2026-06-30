using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class SignalRMessageModel
	{
		public Guid? NotificationId { get; set; }
		public string ContainerId { get; set; } = null!;
		public List<int>? RoleIds { get; set; }
		public int UserId { get; set; }
		public string Message { get; set; }
		public bool NotifyOnStart { get; set; }
	}
}

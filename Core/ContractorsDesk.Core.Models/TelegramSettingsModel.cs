using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class TelegramSettingsModel
	{	
		public string ApiUrl { get; set; } 
		public string BotToken { get; set; }  
		public string ChatId { get; set; }  
		public string ClientName { get; set; }  
		public string DatabaseName { get; set; }
	}
}

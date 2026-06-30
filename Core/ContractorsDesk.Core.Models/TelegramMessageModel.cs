using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class TelegramMessageModel
	{
		[JsonProperty("message_id")]
		public int? MessageId { get; set; }

		[JsonProperty("from")]
		public TelegramUser From { get; set; }

		[JsonProperty("chat")]
		public TelegramChat Chat { get; set; }

		[JsonProperty("date")]
		public int Date { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		public long? ChatId { 			
			get
			{
				return this.Chat != null ? this.Chat.Id : null;
			}
		}
	}
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.Models
{
	public class TelegramApiResponseModel
	{
		[JsonProperty("ok")]
		public bool Ok { get; set; }

		[JsonProperty("error_code")]
		public int ErrorCode { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("result")]
		public TelegramMessageModel Result { get; set; }
	}
	public class TelegramUser
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("is_bot")]
		public bool IsBot { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }
	}

	public class TelegramChat
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}

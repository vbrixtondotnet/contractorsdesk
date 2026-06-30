using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class EmailTemplatePayload
	{
		public Guid Id { get; set; }

		public string? Name { get; set; }

		public string? EmailType { get; set; }

		public string? Body { get; set; }

		public bool IsDefault { get; set; }

		public bool IsNew { get; set; }
	}
}

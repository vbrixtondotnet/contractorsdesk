using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class UserNotificationPayload
	{
		public string Title { get; set; }

		public string Message { get; set; }

		public string Description { get; set; }

		public string RelatedUrl { get; set; }

		public Guid? EmailId { get; set; }

		public int? UserId { get; set; }

		public int? CreatedById { get; set; }
	}
}

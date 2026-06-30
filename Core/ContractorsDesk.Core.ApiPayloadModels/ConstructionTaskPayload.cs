using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ConstructionTaskPayload
	{
		public Guid? Id { get; set; }
		public string Name { get; set; }
		public Guid? ParentTaskId { get; set; }
		public int Sequence { get; set; }
		public int Duration { get; set; }
		public int Pred1Lag { get; set; }
	}
}

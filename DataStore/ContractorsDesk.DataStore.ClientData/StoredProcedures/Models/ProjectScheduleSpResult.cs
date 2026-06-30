using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.DataStore.Client.StoredProcedures.Models
{
	public class ProjectScheduleSpResult
	{
		public Guid? Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid ConstructionTaskId { get; set; }
		public string Name {  get; set; }
		public int Sequence { get; set; }
		public int Duration {  get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public Guid? Pred1 { get; set; }
		public int Lag1 { get; set; }
		public Guid? Pred2 { get; set; }
		public int? Lag2 { get; set; }
		public Guid? Pred3 { get; set; }
		public int? Lag3 { get; set; }

	}
}

using System.Globalization;
namespace ContractorsDesk.Core.Dto
{
	public class ProjectScheduleTaskDto
	{
		public Guid? Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid ConstructionTaskId { get; set; }
		public string Name { get; set; }
		public int Sequence { get; set; }
		public int Duration { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public Guid? Pred1 { get; set; }
		public int Lag1 { get; set; }
		//public Guid? Pred2 { get; set; }
		//public int Lag2 { get; set; }
		//public Guid? Pred3 { get; set; }
		//public int Lag3 { get; set; }
		public string StartDateFormatted
		{
			get
			{
				return StartDate.HasValue ? StartDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty;
			}
		}
		public string EndDateFormatted
		{
			get
			{
				return EndDate.HasValue ? EndDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty;
			}
		}

	}
	public class ProjectScheduleTaskShortDto
	{
		public Guid ConstructionTaskId { get; set; }
		public string Name { get; set; }
		public int Duration { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
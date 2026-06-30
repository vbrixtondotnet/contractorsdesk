namespace ContractorsDesk.Core.Dto
{
	public class SupervisorProjectDto
	{
		public int Id { get; set; }

		public int? SupervisorId { get; set; }

		public Guid? ProjectId { get; set; }

		public DateOnly? DateAssigned { get; set; }
	}
}

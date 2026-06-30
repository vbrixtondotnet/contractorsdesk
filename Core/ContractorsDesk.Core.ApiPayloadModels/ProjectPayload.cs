
using ContractorsDesk.Core.ApiPayloadModels.@base;
namespace ContractorsDesk.Core.ApiPayloadModels
{
    public class ProjectPayload
	{
		public Guid Id {  get; set; }
        public string Name { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
		public string State { get; set; }
		public List<Supervisors>? Supervisors { get; set; }
		public int JobTypeId { get; set; }
		public int StatusId { get; set; }
    }

	public class Supervisors
	{
		public int Id { get; set; }
		public int SupervisorTypeId { get; set; }
	}
}

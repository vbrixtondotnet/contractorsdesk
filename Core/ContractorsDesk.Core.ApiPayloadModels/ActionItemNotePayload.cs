using ContractorsDesk.Core.ApiPayloadModels.@base;

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ActionItemNotePayload
	{
		public Guid? Id { get; set; }
		public int AssignedTo { get;set; } 
		public string? Title { get; set; }
		public string Description { get; set; }
	}
}

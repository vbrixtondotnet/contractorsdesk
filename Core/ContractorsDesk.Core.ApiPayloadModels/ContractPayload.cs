namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class ContractPayload
	{
		public Guid Id { get; set; }

		public string? Name { get; set; }

		public string? BodyTemplate { get; set; }

		public bool IsNew { get; set; }
	}
}

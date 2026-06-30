namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class VendorPayload
	{
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public bool IsNew { get; set; }
        public string? Zip { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Company { get; set; }
    }
}

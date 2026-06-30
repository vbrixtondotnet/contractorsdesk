namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class AudioUploadResponsePayload
	{
		public int? StatusCode { get; set; }
		public string? Status { get; set; }
		public Guid Ref {  get; set; }
		public Guid? ProjectId { get; set; }
		public string? FileUrl { get; set; }
		public string? Transcript { get; set; }
	}
}

namespace ContractorsDesk.Core.ApiPayloadModels
{
	public class AudioUploadUpdateTranscriptPayload
	{
		public Guid Ref { get; set; }
		public string Transcript { get; set; }

		public Guid ProjectId { get; set; }
	}
}

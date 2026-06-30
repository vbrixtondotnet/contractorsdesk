namespace ContractorsDesk.Core.Dto
{
	public class UserUploadDto
	{
		public Guid Id { get; set; }

		public int UserId { get; set; }

		public string FileUrl { get; set; } = null!;

		public string? StorageStatus { get; set; }

		public string? Aistatus { get; set; }

		public DateTime DateCreated { get; set; }
	}
}

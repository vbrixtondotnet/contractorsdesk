using ContractorsDesk.Core.Dto.@base;

namespace ContractorsDesk.Core.Dto
{
	public class UserBookmarkDto
	{
		public Guid Id { get; set; }
		public int UserId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public required string Url { get; set; }
		public ApplicationUserDto User { get; set; }
	}
}

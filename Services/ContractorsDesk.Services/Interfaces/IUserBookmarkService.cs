using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IUserBookmarkService : IBaseService
	{
		Task<List<UserBookmarkDto>> GetUserBookmarksAsync(int userId, string url = "");
		Task<UserBookmarkDto> CreateUserBookmarkAsync(UserBookmarkDto model);
		Task<bool> RemoveBookmark(Guid id);
	}
}

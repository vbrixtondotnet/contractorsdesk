using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services
{
	public class UserBookmarkService : BaseService, IUserBookmarkService
	{
		public UserBookmarkService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext)
		{
			this.mapper = mapper;
		}

		public async Task<UserBookmarkDto> CreateUserBookmarkAsync(UserBookmarkDto model)
		{
			var dbUserBookmark = mapper.Map<UserBookmark>(model);

			if(string.IsNullOrWhiteSpace(dbUserBookmark.Title))
				throw new ArgumentNullException(nameof(dbUserBookmark.Title));

			if (string.IsNullOrWhiteSpace(dbUserBookmark.Url))
				throw new ArgumentNullException(nameof(dbUserBookmark.Title));

			dbUserBookmark.Id = Guid.NewGuid();
			ClientDbContext.UserBookmarks.Add(dbUserBookmark);
			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<UserBookmarkDto>(dbUserBookmark);	
		}

		public async Task<List<UserBookmarkDto>> GetUserBookmarksAsync(int userId, string url = "")
		{
			var dbUserBookmarks = await ClientDbContext.UserBookmarks
				.Where(u => u.UserId == userId)
				.OrderBy(u => u.DateCreated)
				.ToListAsync();

			if (!string.IsNullOrEmpty(url))
			{
				dbUserBookmarks = dbUserBookmarks.Where(b => b.Url.Trim().ToUpper() == url.Trim().ToUpper()).ToList();
			}

			return mapper.Map<List<UserBookmarkDto>>(dbUserBookmarks);
		}

		public async Task<bool> RemoveBookmark(Guid id)
		{
			var dbUserbookmark = await ClientDbContext.UserBookmarks.FirstOrDefaultAsync(u => u.Id == id);

			if (dbUserbookmark == null) throw new ArgumentNullException("User bookmark not found.");

			ClientDbContext.UserBookmarks.Remove(dbUserbookmark);
			await ClientDbContext.SaveChangesAsync();
			return true;
		}
	}
}

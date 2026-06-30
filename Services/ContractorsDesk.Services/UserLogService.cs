using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ContractorsDesk.Services
{
	public class UserLogService : BaseService, IUserLogService
	{
		public UserLogService(ClientDbContext clientDataDbContext, IMapper mapper) : 
			base(mapper, clientDataDbContext) 
		{
			this.ClientDbContext = clientDataDbContext;
			this.mapper = mapper;
		}

		public async Task CreateLogAsync(string path, int userId)
		{
			var userlog = new UserLog();
			userlog.Id = Guid.NewGuid();
			userlog.UserId = userId;
			userlog.Url = path;
			ClientDbContext.UserLogs.Add(userlog);
			await ClientDbContext.SaveChangesAsync();
		}
	}
}

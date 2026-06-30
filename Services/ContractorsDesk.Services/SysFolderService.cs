using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.Services
{
	public class SysFolderService : BaseService, ISysFolderService
	{
		public SysFolderService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext) 
		{

		}
		public async Task<List<SysFolderDto>> GetSysFoldersAsync()
		{
			var dbSysFolders = await ClientDbContext
					.SysFolders
					.AsNoTracking()
					.OrderBy(x => x.Name).ToListAsync();

			return mapper.Map<List<SysFolderDto>>(dbSysFolders);
		}
	}
}

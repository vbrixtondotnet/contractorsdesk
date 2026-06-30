using ContractorsDesk.Core.Dto;
using AutoMapper;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class PermissionMapper : Profile
	{
		public PermissionMapper()
		{
            CreateMap<Permission, PermissionDto>();
		}
	}
}

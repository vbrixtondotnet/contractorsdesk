using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class RoleMapper : Profile
	{
		public RoleMapper()
		{
			CreateMap<Role, RoleDto>()
				.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.RoleType))
				.ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp=> rp.Permission)));

            CreateMap<RoleDto, Role>()
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.CategoryId));
        }
	}
}

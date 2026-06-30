using ContractorsDesk.Core.Dto;
using AutoMapper;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ApplicationUserMapper : Profile
	{
		public ApplicationUserMapper()
		{
			CreateMap<User, ApplicationUserDto>()
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.DateCreated));

			CreateMap<Core.ApiPayloadModels.UserPayload, User>()
				.ForMember(dest => dest.Password, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore());



            CreateMap<User, ApplicationUserShortDetailsDto>();
			CreateMap<User, AccountDetailsDto>();

			CreateMap<ApplicationUserDto, ApplicationUser>();

            CreateMap<User, ApplicationUser>();
        }
	}
}

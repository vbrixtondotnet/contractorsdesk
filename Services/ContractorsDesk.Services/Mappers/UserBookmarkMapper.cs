using ContractorsDesk.Core.Dto;
using AutoMapper;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class UserBookmarkMapper : Profile
	{
		public UserBookmarkMapper()
		{
			CreateMap<UserBookmark, UserBookmarkDto>().ReverseMap();
			CreateMap<Core.ApiPayloadModels.UserBookmark, UserBookmarkDto>();
		}
	}
}

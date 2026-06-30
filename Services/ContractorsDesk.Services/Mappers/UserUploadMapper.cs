using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class UserUploadMapper : Profile
	{
		public UserUploadMapper()
		{
			CreateMap<UserUploadDto, AudioUpload>().ReverseMap();
		}
	}
}

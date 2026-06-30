using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class SysFolderMapper : Profile
	{
		public SysFolderMapper()
		{
			CreateMap<SysFolder, SysFolderDto>()
				.ForMember(dest => dest.Files, opt => opt.Ignore())
				.ForMember(dest => dest.SubFolders, opt => opt.Ignore());

        }
	}
}

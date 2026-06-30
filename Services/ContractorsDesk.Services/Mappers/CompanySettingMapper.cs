using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class CompanySettingMapper : Profile
	{
		public CompanySettingMapper()
		{
			CreateMap<CompanySetting, CompanySettingDto>().ReverseMap();
		}
	}
}

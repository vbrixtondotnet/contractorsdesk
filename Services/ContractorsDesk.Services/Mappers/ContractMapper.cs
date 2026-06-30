using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ContractMapper : Profile
	{
		public ContractMapper()
		{
			CreateMap<Contract, ContractDto>().ReverseMap();
			CreateMap<ContractPayload, Contract>();
		}
	}
}

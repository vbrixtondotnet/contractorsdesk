using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class QbAccountMapper : Profile
	{
		public QbAccountMapper()
		{
			CreateMap<QbAccount, QBAccountDto>();
		}
	}
}

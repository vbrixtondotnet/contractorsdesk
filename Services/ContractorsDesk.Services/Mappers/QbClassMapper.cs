using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class QbClassMapper : Profile
	{
		public QbClassMapper()
		{
			CreateMap<Qbclass, QbClassDto>();
		}
	}
}

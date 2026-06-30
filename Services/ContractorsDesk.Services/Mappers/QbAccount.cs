using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class QbAccount : Profile
	{
		public QbAccount()
		{
			CreateMap<Qbaccount, AccountDto>(); 
			CreateMap<Qbaccount, QBAccountDto>();
		}
	}
}

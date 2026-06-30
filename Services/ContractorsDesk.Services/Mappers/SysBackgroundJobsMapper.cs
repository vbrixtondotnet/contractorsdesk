using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
 

namespace ContractorsDesk.Services.Mappers
{
    public class SysBackgroundJobsMapper : Profile
    {
        public SysBackgroundJobsMapper()
        {
            CreateMap<SysBackgroundJob, SysBackgroundJobsDto>().ReverseMap();
        }
    }
}

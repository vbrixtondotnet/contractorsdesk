using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class ReportsMapper : Profile
	{
		public ReportsMapper()
		{
			CreateMap<ProjectDto, JobsSummaryReportDto>()
				.ForMember(dest => dest.JobAddress, opt => opt.MapFrom(src => src.Name));
        }
	}
}

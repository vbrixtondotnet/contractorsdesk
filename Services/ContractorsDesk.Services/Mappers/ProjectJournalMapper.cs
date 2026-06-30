using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ProjectJournalMapper : Profile
	{
		public ProjectJournalMapper()
		{
			CreateMap<ProjectJournal, ProjectJournalDto>()
				.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(p => p.Project.Name));

			CreateMap<ProjectJournalPayload, ProjectJournal>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Journal, opt => opt.Ignore());
		}
	}
}

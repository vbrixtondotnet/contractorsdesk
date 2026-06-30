using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ProjectNoteMapper : Profile
	{
		public ProjectNoteMapper()
		{

			CreateMap<ProjectNote, ProjectNoteDto>()
				.ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByNavigation))
				.ForMember(dest => dest.UpdatedByUser, opt => opt.MapFrom(src => src.UpdatedByNavigation));

			CreateMap<ProjectNoteModel, ProjectNote>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        }
	}
}

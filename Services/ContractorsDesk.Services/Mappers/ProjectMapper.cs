using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services.Mappers
{
	public class ProjectMapper : Profile
	{
		public ProjectMapper()
		{
			CreateMap<EstimateHistory, EstimateHistoryDto>();
			CreateMap<EstimateMapping, EstimateMappingDto>();
			CreateMap<Estimate, EstimateDto>();
			CreateMap<Qbtransaction, QBTransactionDto>();
			CreateMap<ProjectDocumentDto, ProjectDocument>().ReverseMap();

			CreateMap<Qbclass, ProjectDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Qbaccount));

			CreateMap<Qbclass, ProjectDetailsDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.FullyQualifiedName, opt => opt.MapFrom(src => src.FullyQualifiedName))
				.ReverseMap();

			CreateMap<ProjectClientDto, Qbcustomer>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
				.ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.ListId, opt => opt.MapFrom(src => "NonQBOCustomer"));

			CreateMap<ProjectClientPayload, ProjectClientDto>()
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore());

			CreateMap<ProjectSupervisor, ProjectSupervisorDto>()
				.ForMember(dest => dest.Supervisor, opt => opt.Ignore());

			CreateMap<ActiveProjectDto, InboxProjectDto>()
				.ForMember(dest => dest.UnreadCount, opt => opt.Ignore());
		}
	}
}

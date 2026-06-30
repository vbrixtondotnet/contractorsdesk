using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class EstimateCategoryMapper : Profile
	{
		public EstimateCategoryMapper()
		{
			CreateMap<EstimateCategory, EstimateCategoryDto>()
				.ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.ParentEstimateCategory));

			CreateMap<EstimateCategoryDto, EstimateCategory>();
		}
	}
}

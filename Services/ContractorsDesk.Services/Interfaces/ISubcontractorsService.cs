using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface ISubcontractorsService : IBaseService
	{
		Task<List<SubContractorDto>> GetSubContractorsByProjectIdAsync(Guid projectId, bool excludeAddedSubcontractors = false);
		Task<List<SubContractorDto>> GetAllSubcontractorsAsync(string? searchKey = null);
		Task<SysBackgroundJobsDto> GetSubcontractorByIdAsync(Guid Id);
        Task<List<string>> GetSubcontractorTypesAsync();
        Task<SubContractorProjectDto?> GetSubContractorWithProjectsAsync(Guid id);
        Task<SubContractorDto> CreateSubcontractorAsync(SubContractorPayloadModel model);
		Task<SubContractorDto> UpdateSubcontractorAsync(SubContractorPayloadModel model);
        Task<SubContractorDto> CreateSubcontractorAsync(SubContractorDto subContractorDto);
        Task UpdateSubcontractorAsync(SubContractorDto subContractorDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteProjectSubContractorAsync(Guid id, Guid projectId);
    }
}

using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services
{
	public class SubcontractorsService : BaseService, ISubcontractorsService
	{
        private readonly IProjectsService projectsService;
        public SubcontractorsService(ClientDbContext clientDbContext, IMapper mapper, IProjectsService projectsService)
			: base(mapper, clientDbContext) 
		{
            this.projectsService = projectsService;
		}
		public async Task<List<SubContractorDto>> GetSubContractorsByProjectIdAsync(Guid projectId, bool excludeAddedSubcontractors = false)
        {
            var retval = new List<SubContractor>();

			if (excludeAddedSubcontractors)
            {
				retval = await ClientDbContext.ProjectSubContractors
			         .AsNoTracking()
			         .OrderBy(x => x.SubContractor.Name)
			         .Select(x => x.SubContractor)
			         .ToListAsync();

				var addedSubContractors = await ClientDbContext.ProjectSubContractors
					.AsNoTracking()
					.Where(x => x.ProjectId == projectId)
					.Select(x => x.SubContractorId)
					.ToListAsync();

                retval = retval.Where(x => !addedSubContractors.Contains(x.Id)).DistinctBy(x=> x.Id).ToList();
			}
            else
            {
				retval = await ClientDbContext.ProjectSubContractors
					 .AsNoTracking()
					 .Where(x => x.ProjectId == projectId)
					 .OrderBy(x => x.SubContractor.Name)
					 .Select(x => x.SubContractor)
					 .ToListAsync();

				retval = retval.DistinctBy(x => x.Id).ToList();
			}

			return mapper.Map<List<SubContractorDto>>(retval);
		}
		public async Task<List<SubContractorDto>> GetAllSubcontractorsAsync(string? searchKey = null)
		{
			if(!string.IsNullOrEmpty(searchKey) && searchKey.Length > 2)
			{
				return await GetAllSubcontractorsByKeywordAsync(searchKey);
			}

            var subcontractors = await ClientDbContext.SubContractors
                 .AsNoTracking()
                 .Where(x => x.IsActive == true)
                 .OrderBy(x => x.Name)
                 .ToListAsync();

            return mapper.Map<List<SubContractorDto>>(subcontractors);
		}
        public async Task<SysBackgroundJobsDto> GetSubcontractorByIdAsync(Guid Id)
        {
            var subContractor = await ClientDbContext.SubContractors.FindAsync(Id);
            return mapper.Map<SysBackgroundJobsDto>(subContractor);
        }
        public async Task<List<string>> GetSubcontractorTypesAsync()
        {
            var categories = await ClientDbContext.SubContractorCategories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            return categories;
        }

        public async Task<SubContractorDto> CreateSubcontractorAsync(SubContractorPayloadModel model)
		{
			var subcontractor = mapper.Map<SubContractor>(model);

			subcontractor.Id = Guid.NewGuid();
			ClientDbContext.SubContractors.Add(subcontractor);

			if (model.ProjectId != null)
			{
				var projectSubContractor = new ProjectSubContractor
				{
					Id = Guid.NewGuid(),
					ProjectId = model.ProjectId.Value,
					SubContractorId = subcontractor.Id
				};

				ClientDbContext.ProjectSubContractors.Add(projectSubContractor);
			}

			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<SubContractorDto>(subcontractor);
		}
		public async Task<SubContractorDto> UpdateSubcontractorAsync(SubContractor model)
		{
			var subcontractor = await ClientDbContext.SubContractors
				.FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new Exception("Subcontractor does not exist.");

			subcontractor = mapper.Map(model, subcontractor);

			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<SubContractorDto>(subcontractor);
		}
        public async Task<SubContractorDto> CreateSubcontractorAsync(SubContractorDto subContractorDto)
        {
            var subcontractor = mapper.Map<SubContractor>(subContractorDto);

            subcontractor.Id = Guid.NewGuid();
            ClientDbContext.SubContractors.Add(subcontractor);

            if (subContractorDto.ProjectId != null)
            {
                var projectSubContractor = new ProjectSubContractor
                {
                    Id = Guid.NewGuid(),
                    ProjectId = subContractorDto.ProjectId.Value,
                    SubContractorId = subcontractor.Id
                };

                ClientDbContext.ProjectSubContractors.Add(projectSubContractor);
            }
            await ClientDbContext.SaveChangesAsync();

            return mapper.Map<SubContractorDto>(subcontractor);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var subcontractor = await ClientDbContext.SubContractors
                 .FirstOrDefaultAsync(v => v.Id == id);

            if (subcontractor == null)
                return false;

            subcontractor.IsActive = false;

            ClientDbContext.SubContractors.Update(subcontractor);
            await ClientDbContext.SaveChangesAsync();

            return true;
        }
        public async Task UpdateSubcontractorAsync(SubContractorDto subContractorDto)
        {
            if (subContractorDto == null)
                throw new ArgumentNullException(nameof(subContractorDto));

            var subcontractor = await ClientDbContext.SubContractors
                .FirstOrDefaultAsync(x => x.Id == subContractorDto.Id)
                ?? throw new Exception("Subcontractor does not exist.");

            // Map updated fields from DTO to entity
            mapper.Map(subContractorDto, subcontractor);

            await ClientDbContext.SaveChangesAsync();
        }
	    public async Task<SubContractorDto> UpdateSubcontractorAsync(SubContractorPayloadModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var subcontractor = await ClientDbContext.SubContractors
                .FirstOrDefaultAsync(x => x.Id == model.Id)
                ?? throw new Exception("Subcontractor does not exist.");

            // Map updated fields from model to entity
            mapper.Map(model, subcontractor);

            await ClientDbContext.SaveChangesAsync();

            return mapper.Map<SubContractorDto>(subcontractor);
        }
        public async Task<SubContractorProjectDto?> GetSubContractorWithProjectsAsync(Guid id)
        {
            var subContractor = await ClientDbContext.SubContractors
                .Where(sc => sc.Id == id)
                .Select(sc => new SubContractorProjectDto
                {
                    Id = sc.Id,
                    DateCreated = sc.DateCreated,
                    CreatedBy = sc.CreatedBy,
                    DateUpdated = sc.DateUpdated,
                    UpdatedBy = sc.UpdatedBy,
                    Projects = new List<ProjectDetailsDto>()
                })
                .FirstOrDefaultAsync();

            if (subContractor == null)
                return null;

            var projectIds = await ClientDbContext.ProjectSubContractors
                .Where(psc => psc.SubContractorId == id)
                .Select(psc => psc.ProjectId)
                .ToListAsync();

            var projectList = new List<ProjectDetailsDto>();
            foreach (var projectId in projectIds)
            {
                var projectDetails = await projectsService.GetProjectShortDetailsAsync(projectId);
                if (projectDetails != null)
                {
                    projectList.Add(projectDetails);
                }
            }

            subContractor.Projects = projectList;
            return subContractor;
        }
        public async Task<bool> DeleteProjectSubContractorAsync(Guid id, Guid projectId)
        {
            var subcontractor = await ClientDbContext.ProjectSubContractors
                .FirstOrDefaultAsync(v => v.SubContractorId == id && v.ProjectId == projectId);

            if (subcontractor == null)
                return false;

            ClientDbContext.ProjectSubContractors.Remove(subcontractor);
            await ClientDbContext.SaveChangesAsync();

            return true;
        }

        private async Task<List<SubContractorDto>> GetAllSubcontractorsByKeywordAsync(string searchKey)
		{
			var subcontractors = await ClientDbContext.SubContractors
				.AsNoTracking()
				 .Where(c => c.Name.Contains(searchKey) ||
							c.Email.Contains(searchKey))
				.OrderBy(x => x.Name)
				.ToListAsync();

			return mapper.Map<List<SubContractorDto>>(subcontractors);
		}
	}
}

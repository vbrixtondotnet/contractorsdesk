using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ContractorsDesk.Services
{
    public class ProposalService : BaseService, IProposalService
	{
		private readonly IApplicationUserService userService;
		private readonly IPostMarkEmailService emailService;
		private readonly IProposalTemplatesService proposalTemplatesService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;
		private readonly ICacheService cacheService;

        public ProposalService(
			ClientDbContext clientDataDbContext, 
			IMapper mapper,
			IApplicationUserService userService,
            IConfiguration configuration,
            IPostMarkEmailService emailService,
			IProposalTemplatesService proposalTemplatesService,
			ICacheService cacheService,
			IHttpContextAccessor httpContextAccessor
            )
			:base(mapper, clientDataDbContext)
		{
			this.userService = userService;
			this.emailService = emailService;
			this.cacheService = cacheService;
			this.proposalTemplatesService = proposalTemplatesService;
            this.httpContextAccessor = httpContextAccessor;
			this.configuration = configuration;

			this.cacheService.UserId = this.UserId;
        }

		#region Public Methods
		public async Task<List<ProposalManagementDto>> GetProposalsAsync(int? supervisorId, bool isArchived = false)
		{
			var proposals = await GetProposalsBySupervisorIdAsync(supervisorId, isArchived);

			var retval = MapProposalManagementsToDto(proposals);

			return retval.OrderByDescending(p => p.DateUpdated).ToList();
		}
		public async Task<ProposalDto> GetProposalAsync(Guid id, bool includeZeroAmounts = true)
		{
			var dbProposal = await LoadProposalWithRelatedDataAsync(id);
			var template = await GetTemplate();
			var proposal = await MapProposalDtoAsync(dbProposal, id, includeTemplate: true, includeZeroAmounts);
			var projectSchedule = await ClientDbContext.ProjectSchedules
				.AsNoTracking()
				.FirstOrDefaultAsync(ps => ps.ProjectId == dbProposal.QbclassId);

			proposal.HasProjectSchedule = projectSchedule != null ? true : false;

			return proposal;
		}
		public async Task<bool> CheckIfProposalIncludesZeroAmount(Guid id)
		{ 
			var dbProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
			if(dbProposal == null) throw new Exception("Proposal not found.");

            return dbProposal.IncludeLinesWithZeroAmount;
		}
		public async Task<ProposalDto> GetProposalDetailsAsync(Guid id)
		{
			var dbProposal = await LoadProposalWithRelatedDataAsync(id);
			return await MapProposalDtoAsync(dbProposal, id, includeTemplate: false);
		}
		public async Task<List<ProposalLineDto>> GetProposalLinesAsync(Guid proposalId) 
		{
			var retval = new List<ProposalLineDto>();
			var dbProposalLines = await ClientDbContext.ProposalLines
				.Where(pl => pl.ProposalId == proposalId)
				.OrderBy(pl => pl.Sequence)
				.ToListAsync();

			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p=> p.Id == proposalId);
			if (proposal == null) throw new Exception("Proposal not found.");

			var template = await ClientDbContext.ProposalTemplates
				.Include(t => t.ProposalTemplatesLineItems)
				.FirstOrDefaultAsync(t => t.Id == proposal.TemplateId);

			if (template == null) throw new Exception("Proposal Template not found.");

			var parentCategoriesFromTemplate = template.ProposalTemplatesLineItems.Where(p=> p.ParentId == null).ToList();

			var parentCategories = dbProposalLines.Select(pl => pl.ParentEstimateCategoryId).Distinct().ToList();
			var dbParentCategories = await ClientDbContext.EstimateCategories
				.Where(ec => parentCategories.Contains(ec.Id))
				.ToListAsync();

			foreach (var dbProposalLine in dbProposalLines)
			{
				var parentFromTemplate = parentCategoriesFromTemplate.FirstOrDefault(p => p.EstimateCategoryId == dbProposalLine.ParentEstimateCategoryId);
				var parent = dbParentCategories.FirstOrDefault(ec => ec.Id == dbProposalLine.ParentEstimateCategoryId);
				var proposalLineDto = new ProposalLineDto
				{
					EstimateCategoryId = dbProposalLine.EstimateCategoryId,
					Name = dbProposalLine.Name,
					ParentEstimateCategoryId = dbProposalLine.ParentEstimateCategoryId.Value,
					Sequence = dbProposalLine.Sequence,
				};

				proposalLineDto.ParentName = parentFromTemplate != null ? parentFromTemplate.Name : parent.Name;
				proposalLineDto.ParentSequence = parentFromTemplate != null ? parentFromTemplate.Sequence : parent.Sequence;
				retval.Add(proposalLineDto);
			}

			retval = retval.OrderBy(p => p.ParentSequence).ThenBy(p => p.Sequence).ToList();

			return retval;
		}
		public async Task<ProposalDto> NewProposalAsync()
		{
			//var retval = await cacheService.GetFromCache<ProposalDto>("NewProposal");

			//if (retval == null)
			//{
				var template = await GetTemplate();
				var proposal = new ProposalDto();
				proposal.Date = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().ToShortDateString();
				proposal.Number = ClientDbContext.Proposals.Count() + 1;
				proposal.Status = (int)DocStatus.Draft;
				proposal.Template = mapper.Map<ProposalTemplateDto>(template);
				proposal.Template.Categories = await this.proposalTemplatesService.GetLineItemsAsync(template.Id);

				//await cacheService.SetAsync("NewProposal", proposal);

				return proposal;
			//}

			//return retval;
		}
		public async Task<ProposalDto> SaveProposalAsync(ProposalModel model, bool merge = false, bool overwrite = false)
		{
			var newStatus = ((DocStatus)model.Status).GetStringValue();
			var previousStatus = newStatus;

			if(model.Id != Guid.Empty)
			{
				var dbProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p=> p.Id == model.Id) ?? throw new ArgumentException("Proposal not found.");
				previousStatus = dbProposal.DocStatus;
			}

			var proposal = await CreateOrUpdateProposalAsync(model);

			// Project Details
			if(model.Project != null)
			await UpdateProjectDetailsAsync(model, proposal);

			// Client Details
			if(model.Client != null)
				await UpdateProposalClientAsync(model, proposal);

			if(model.Supervisors != null && model.Supervisors.Any())
				// Supervisors
				await UpdateProposalSupervisorAsync(model, proposal);

			// Estimate Categories
			await PopulateProposalLinesAsync(model, proposal);

			await ClientDbContext.SaveChangesAsync();

			if (merge)
			{
				// merge
				var replacedProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == model.MergeWithProjectId && p.IsDeleted != true);
				if (replacedProposal != null)
				{
					replacedProposal.IsDeleted = true;
				}

				var currentProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == proposal.Id);
				if (currentProposal != null)
				{
					currentProposal.QbclassId = model.MergeWithProjectId;
					currentProposal.DocStatus = DocStatus.Accepted.GetStringValue();
				}
			}
			else if (overwrite)
			{
				var overwriteProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == model.OverwriteProposalId);
				overwriteProposal.IsDeleted = true;
			}

			await HandleProposalChangeStatus(previousStatus, newStatus, proposal);

			await ClientDbContext.SaveChangesAsync();

			return await GetProposalAsync(proposal.Id);
		}	
		public async Task<Proposal> UpdateStatusAsync(UpdateProposalStatusPayload payload)
		{
			var dbProposal = await ClientDbContext.Proposals
					.Include(p=> p.Qbclass)
					.Include(p=> p.Client)
					.Include(p=> p.ProposalProject)
					.FirstOrDefaultAsync(p => p.Id == payload.ProposalId);

			if (dbProposal == null) throw new ArgumentException($"Proposal not found");

			var previousStatus = dbProposal.DocStatus;
			var newStatus = payload.DocStatus;
			var proposalClient = dbProposal.Client;
			var status = previousStatus == "Accepted" ? previousStatus : newStatus;

			dbProposal.DocStatus = status;

			await HandleProposalChangeStatus(previousStatus, newStatus, dbProposal);

			await ClientDbContext.SaveChangesAsync();

			return dbProposal;
		}
		public async Task<bool> CheckActiveProjectAsync(string projectName)
		{
			var existingProject = await ClientDbContext.Qbclasses
				.Where(p=> p.FullyQualifiedName.ToUpper().Trim() == projectName.ToUpper().Trim() 
							&& (p.ActiveJobs == true || p.ActiveSpecJobs == true))
				.FirstOrDefaultAsync();

			return existingProject != null;
		}
		public async Task<List<ProposalTemplateLineItemDto>> GetLineItemsByNameAndTemplateAsync(Guid proposalTemplateId, string name)
		{
			var proposalTemplate = await ClientDbContext.ProposalTemplates
				.Include(t=> t.ProposalTemplatesLineItems)
				.FirstOrDefaultAsync(t=> t.Id == proposalTemplateId);

			if (proposalTemplate == null) throw new Exception("Proposal Template does not exist.");

			var lineItems = proposalTemplate.ProposalTemplatesLineItems.Where(l => l.Name.ToUpper().Trim() == name.ToUpper().Trim()).ToList();
			return mapper.Map<List<ProposalTemplateLineItemDto>>(lineItems);
		}
		public async Task<bool> IsLineItemAdded(Guid proposalId, string name)
		{
			var proposalLine = await ClientDbContext.ProposalLines
				.FirstOrDefaultAsync(pl => pl.ProposalId == proposalId && pl.Name.ToUpper().Trim() == name.ToUpper().Trim());

			return proposalLine != null;
		}
		public async Task<bool> DeleteProposal(Guid id)
		{
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p=> p.Id == id) 
				?? throw new ArgumentException("Proposal not found.");
			
			proposal.IsDeleted = true;
			await ClientDbContext.SaveChangesAsync();
			return true;
		}
		public async Task<List<ProposalCsvDto>> GetProposalLinesForExportGroupedAsync(Guid id)
        {
            var lines = await ClientDbContext.ProposalLines
				.Where(pl => pl.ProposalId == id)
				.AsNoTracking()
				.OrderBy(pl => pl.Sequence)
				.ToListAsync();

            var csvList = new List<ProposalCsvDto>();

            // Parents: those with no ParentEstimateCategoryId
            var parents = lines
                .Where(pl => pl.ParentEstimateCategoryId == null)
                .OrderBy(pl => pl.Sequence)
                .ToList();

            foreach (var parent in parents)
            {
                // Add the parent row
                csvList.Add(new ProposalCsvDto
                {
                    Sequence = parent.Sequence,
                    Name = parent.Name,
                    Description = parent.Description,
                    Amount = 0
                });

                // Find children whose ParentEstimateCategoryId == parent's EstimateCategoryId
                var children = lines
                    .Where(pl => pl.ParentEstimateCategoryId == parent.EstimateCategoryId)
                    .OrderBy(pl => pl.Sequence)
                    .ToList();

                foreach (var child in children)
                {
                    csvList.Add(new ProposalCsvDto
                    {
                        Sequence = child.Sequence,
                        Name = child.Name,
                        Description = child.Description,
                        Amount = child.Amount
                    });
                }
            }

            return csvList;
        }
        public async Task UpdateProposalIncludeZeroAmount(Guid id)
        {
            var dbProposal = await ClientDbContext.Proposals
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProposal == null) throw new ArgumentException($"Unable to find proposal with and Id of {id}");
            dbProposal.IncludeLinesWithZeroAmount = !dbProposal.IncludeLinesWithZeroAmount;
            await ClientDbContext.SaveChangesAsync();
        }
		public async Task<List<ProjectMatchResultDto>?> GetMatchingProjectsByNameAsync(string projectName, Guid? id = null)
		{
			var projectNameLower = projectName.ToLower();
			var results = await ClientDbContext.Qbclasses
									.Where(q => q.Name != null && (q.Name.ToLower().StartsWith(projectNameLower) || projectNameLower.StartsWith(q.Name.ToLower())))
									.Select(q => new ProjectMatchResultDto { Id = q.Id, Name = q.Name })
									.ToListAsync();
			if(id != null)
			{
				results = results.Where(q => q.Id != id).ToList();
			}

			return results.Any() ? results : null;
        }

		public async Task<List<ProjectMatchResultDto>?> GetMatchingDraftProposalsByNameAsync(string projectName, Guid? id = null)
		{
			var projectNameLower = projectName.ToLower();
			var results = await ClientDbContext.Proposals
								.Include(p=> p.ProposalProject)
								.Where(p=> p.DocStatus == "Draft" && !p.IsArchived)
								.Where(p => p.ProposalProject.Name != null && (p.ProposalProject.Name.ToLower().StartsWith(projectNameLower) || projectNameLower.StartsWith(p.ProposalProject.Name.ToLower())))
								.Select(q => new ProjectMatchResultDto { Id = q.Id, Name =	q.ProposalProject.Name })
								.ToListAsync();
			if (id != null)
			{
				results = results.Where(q => q.Id != id).ToList();
			}

			return results.Any() ? results : null;
		}

		public async Task<bool> ValidateClientEmailAddress(ClientModel model)
		{
			var existingEmailAddressError = "Unable to save proposal. A client with the same email address already exists.";
			var retval = true;
			Client? client = null;

			if (model.Id != null)
			{
				client = ClientDbContext.Clients
					.Where(c => c.Id != model.Id.Value && c.EmailAddress.ToLower().Trim() == model.EmailAddress.ToLower().Trim())
					.AsNoTracking().FirstOrDefault();
			}
			else
			{
				client = ClientDbContext.Clients
					.Where(c => c.EmailAddress.ToLower().Trim() == model.EmailAddress.ToLower().Trim())
					.AsNoTracking().FirstOrDefault();
			}

			return client == null;
		}

		#endregion

		#region Private Methods
		private async Task<Proposal> CreateOrUpdateProposalAsync(ProposalModel model)
		{
            Proposal proposal;
            if (model.Id == Guid.Empty)
            {
                proposal = new Proposal();
                proposal.Id = Guid.NewGuid();
                proposal.Number = model.Number;
                proposal.Date = Convert.ToDateTime(model.Date);
                proposal.DocStatus = DocStatus.Draft.GetStringValue();
                proposal.TemplateId = model.Template.Id;
                proposal.TotalAmount = model.TotalAmount;
                ClientDbContext.Proposals.Add(proposal);
            }
            else
            {
                proposal = await ClientDbContext.Proposals
                .FirstOrDefaultAsync(p => p.Id == model.Id) ?? throw new ArgumentException("Proposal not found.");
            }

            proposal.Date = Convert.ToDateTime(model.Date);
            proposal.TotalAmount = model.TotalAmount;
			proposal.DocStatus = ((DocStatus)model.Status).GetStringValue();

			return proposal;
        }
		private async Task UpdateProjectDetailsAsync(ProposalModel model, Proposal dbProposal)
		{
			// Project Details
			var existingProposalProject = await ClientDbContext.ProposalProjects
				.FirstOrDefaultAsync(pp => pp.Id == dbProposal.ProposalProjectId);

			if (existingProposalProject != null)
			{
				mapper.Map(model.Project, existingProposalProject);
			}
			else
			{
				var proposalProject = mapper.Map<ProposalProject>(model.Project);
				proposalProject.Id = Guid.NewGuid();
				dbProposal.ProposalProjectId = proposalProject.Id;
				ClientDbContext.ProposalProjects.Add(proposalProject);
			}
		}
		private async Task UpdateProposalClientAsync(ProposalModel model, Proposal dbProposal)
		{
			var clientModel = model.Client;
			var existingEmailAddressError = "Unable to save proposal. A client with the same email address already exists.";

			var isValidClient = await ValidateClientEmailAddress(clientModel);
			if (isValidClient)
			{
				if (clientModel.Id != null)
				{
					var dbClient = await ClientDbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientModel.Id.Value);

					mapper.Map(model.Client, dbClient);
					dbProposal.ClientId = dbClient.Id;
				}
				else
				{
					var newClient = mapper.Map<Client>(clientModel);
					newClient.Id = Guid.NewGuid();
					dbProposal.ClientId = newClient.Id;
					ClientDbContext.Clients.Add(newClient);
				}
			}
			else
			{
				throw new Exception(existingEmailAddressError);
			}
            
        }
		private async Task UpdateProposalSupervisorAsync(ProposalModel model, Proposal dbProposal)
		{
			var existingSupervisors = ClientDbContext.ProposalSupervisors.Where(ps => ps.ProposalId == dbProposal.Id);
			ClientDbContext.ProposalSupervisors.RemoveRange(existingSupervisors);

			ClientDbContext.ProposalSupervisors.AddRange(
			model.Supervisors.Select(id => new ProposalSupervisor
			{
				Id = Guid.NewGuid(),
				ProposalId = dbProposal.Id,
				SupervisorTypeId = (int)SupervisorType.ProjectManager,
				UserId = id
			})
		);
		}
		private async Task HandleProposalChangeStatus(string previousStatus, string newStatus, Proposal proposal)
		{
			var projectId = Guid.NewGuid();

            if (previousStatus != DocStatus.Accepted.GetStringValue() && newStatus == DocStatus.Accepted.GetStringValue())
			{
				var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(c => c.Id == proposal.QbclassId);
				if(qbClass != null)
				{
					qbClass.ActiveJobs = true;
					qbClass.IsArchived = false;
					qbClass.IsActive = true;
					projectId = qbClass.Id;

                }
				else
				{
					var className = proposal.ProposalProject != null ? proposal.ProposalProject.Name : "New Project";
					qbClass = new Qbclass
					{
						Id = projectId,
						FullyQualifiedName = className,
						Name = className,
						ActiveJobs = true,
						IsActive = true,
						ListId = "NonQBOProject",
						TimeCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
						CreatedBy = this.UserId.ToString(),
						AllowedForJobReports = true,
						OpenedDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()
					};
					ClientDbContext.Qbclasses.Add(qbClass);
					proposal.QbclassId = qbClass.Id;
                }

                // Create User Invite here
                await AddClientUser(proposal, projectId);
            }

		}
		private async Task<Proposal?> LoadProposalWithRelatedDataAsync(Guid id)
		{
			return await ClientDbContext.Proposals
				.Include(p => p.Client)
				.Include(p=> p.ProposalProject)
				.Include(p => p.Qbclass)
					.ThenInclude(c => c.ProjectSupervisors)
				.Include(p => p.ProposalSupervisors)
					.ThenInclude(ps => ps.User)
				.Include(p => p.Template)
					.ThenInclude(c => c.ProposalTemplatesLineItems)
				.Include(p => p.ProposalLines)				
				.FirstOrDefaultAsync(p => p.Id == id);
		}
		private List<ProposalManagementDto> MapProposalManagementsToDto(List<Proposal> proposals)
		{
			var retval = new List<ProposalManagementDto>();
			var supervisorIds = proposals.SelectMany(p => p.ProposalSupervisors).Select(p => p.UserId).ToList();
			var users = ClientDbContext.Users.Where(u => supervisorIds.Contains(u.Id)).ToList();

			foreach (var proposal in proposals)
			{
				var project = proposal.ProposalProject;
				var client = proposal.Client;
				var qbClass = proposal.Qbclass;

				var proposalManagementDto = mapper.Map<ProposalManagementDto>(proposal);
				var proposalSupervisors = proposal.ProposalSupervisors.Select(p => p.User).ToList();

				proposalManagementDto.Supervisors = mapper.Map<List<ApplicationUserShortDetailsDto>>(proposalSupervisors);

				if(project != null)
				{
					proposalManagementDto.Project = project.Name;
					proposalManagementDto.ProjectId = project.Id;
				}
				if (client != null)
				{
					proposalManagementDto.Client = client.Name;
					proposalManagementDto.ClientEmailAddress = client.EmailAddress;
				}

				if (qbClass != null)
				{
					proposalManagementDto.QbClassId = qbClass.Id;
				}

				proposalManagementDto.Template = proposal.Template?.Name;
				proposalManagementDto.IsArchived = proposal.IsArchived;

				retval.Add(proposalManagementDto);
			}

			return retval;
		}
		private async Task<ProposalDto> MapProposalDtoAsync(Proposal dbProposal, Guid id, bool includeTemplate, bool includeZeroAmounts = true)
		{
			DocStatus statusEnum = EnumExtensions.GetEnumValueFromString<DocStatus>(dbProposal.DocStatus);
			var client = mapper.Map<ClientDto>(dbProposal.Client);
			var project = mapper.Map<ProposalProjectDto>(dbProposal.ProposalProject);
			var supervisors = dbProposal.ProposalSupervisors.Select(ps => ps.UserId).ToList();

			if(dbProposal.QbclassId != null)
			{
				project.Id = dbProposal.QbclassId.Value;
			}

			var proposal = new ProposalDto
			{
				Date = dbProposal.Date.ToShortDateString(),
				Number = dbProposal.Number,
				Status = (int)statusEnum,
				Client = client,
				Project = project,
				Supervisors = supervisors,
				Id = id,
				QbClassId = dbProposal.QbclassId,
				IncludeLinesWithZeroAmount = dbProposal.IncludeLinesWithZeroAmount
			};

			if (includeTemplate)
			{
				var categories = await GetProposalEstimateCategories(dbProposal, includeZeroAmounts);
				if (dbProposal.Template != null)
				{
					proposal.Template = mapper.Map<ProposalTemplateDto>(dbProposal.Template);
				}
				else
				{
					proposal.Template = new ProposalTemplateDto();
				}

				var nonOverheadCategories = categories.Where(c => c.Name.ToUpper() != "OVERHEAD").ToList();
				var overheadCategory = categories.FirstOrDefault(c => c.Name.ToUpper() == "OVERHEAD");
				proposal.Template.Categories = nonOverheadCategories;
				if (overheadCategory != null)
				{
					proposal.Template.Categories.Add(overheadCategory);
				}

				var lineItemsWithPercentages = proposal.Template.Categories.SelectMany(c => c.LineItems)
					.Where(li => li.Percentage != null && li.Percentage > 0)
					.ToList();
			}

			return proposal;
		}
		private async Task<List<ProposalTemplateLineItemDto>> GetProposalEstimateCategories(Proposal dbProposal, bool includeZeroAmounts = true)
		{
			var retval = new List<ProposalTemplateLineItemDto>();
			var proposalLines = dbProposal.ProposalLines.Where(pl => includeZeroAmounts || pl.Amount > 0);
			var dbCategories = dbProposal.ProposalLines.Where(pl => pl.ParentEstimateCategoryId == null).OrderBy(pl => pl.Sequence);

            foreach (var dbParentCategory in dbCategories)
			{
				var parentCategory = mapper.Map<ProposalTemplateLineItemDto>(dbParentCategory);
				parentCategory.EstimateCategoryId = dbParentCategory.EstimateCategoryId;
				parentCategory.Sequence = dbParentCategory.Sequence == null ? 0 : dbParentCategory.Sequence.Value;
				parentCategory.Id = parentCategory.EstimateCategoryId.Value;

				var lineItems = proposalLines.Where(pl => pl.ParentEstimateCategoryId == parentCategory.EstimateCategoryId).OrderBy(pl => pl.Sequence).ToList();
				parentCategory.LineItems = mapper.Map<List<ProposalTemplateLineItemDto>>(lineItems);

				retval.Add(parentCategory);
			}

            // Assign Sequence according to its index in the retval array
            for (int i = 0; i < retval.Count; i++)
			{ 
				if(retval[i].Name.ToUpper() == "OVERHEAD")
				{
					retval[i].Sequence = retval.Count;
				}
			}

			retval = retval.OrderBy(c => c.Sequence).ToList();

			return retval;
		}

		// Private method to get project details from QbClass if needed
		private async Task<ProposalProjectDetailsDto> GetProjectDetailsFromQbClassAsync(Guid? qbClassId)
		{
			if (qbClassId == null)
				return null;

			var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(c => c.Id == qbClassId);
			return qbClass != null ? new ProposalProjectDetailsDto { Name = qbClass.FullyQualifiedName } : null;
		}
		private async Task<List<ProposalTemplateLineItemDto>> GetProposalLineItemCategoriesAsync(Guid proposalId, bool includeZeroAmounts = true)
		{
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == proposalId);
			if (proposal == null) throw new Exception("Proposal not found.");

			var template = await ClientDbContext.ProposalTemplates
				.Include(t => t.ProposalTemplatesLineItems)
				.FirstOrDefaultAsync(t => t.Id == proposal.TemplateId);

			if (template == null) throw new Exception("Proposal Template not found.");

			var result = ClientDbContext.ProposalLines
						.Where(pl => pl.ProposalId == proposalId && (includeZeroAmounts || pl.Amount > 0))
						.GroupJoin(ClientDbContext.EstimateCategories,
							pl => pl.EstimateCategoryId,
							e => e.Id,
							(pl, eGroup) => new { pl, eGroup })
						.SelectMany(
							x => x.eGroup.DefaultIfEmpty(),
							(x, e) => new { x.pl, e })
						.GroupJoin(ClientDbContext.EstimateCategories,
							x => x.pl.ParentEstimateCategoryId,
							ep => ep.Id,
							(x, epGroup) => new { x.pl, x.e, epGroup })
						.SelectMany(
							x => x.epGroup.DefaultIfEmpty(),
							(x, ep) => new
							{
								EstimateCategoryID = x.pl.EstimateCategoryId,
								ParentEstimateCategoryID = x.pl.ParentEstimateCategoryId,
								Name = x.pl.Name,
								Category = ep != null ? ep.Name : null,
								Description = x.pl.Description,
								Amount = x.pl.Amount,
								Sequence = x.pl.Sequence,
								ParentSequence = ep != null ? ep.Sequence : (int?)null,
								Percentage = x.pl.Percentage
							})
						.OrderBy(result => result.ParentSequence)
						.ThenBy(result => result.Sequence)
						.ToList();

			var dbCategories = result
				.GroupBy(parent => new { parent.ParentEstimateCategoryID, parent.Category, parent.ParentSequence })
				.Select(r=> new { ID = r.Key.ParentEstimateCategoryID, Name = r.Key.Category, Sequence = r.Key.ParentSequence })
				.OrderBy(r => r.Sequence)
				.Distinct()
				.ToList();

			var parentCategoriesFromTemplate = template.ProposalTemplatesLineItems.Where(p => p.ParentId == null).ToList();

			var retval = new List<ProposalTemplateLineItemDto>();
			foreach (var category in dbCategories)
			{
				var parentFromTemplate = parentCategoriesFromTemplate.FirstOrDefault(p => p.EstimateCategoryId == category.ID.Value);

				var proposalTemplateLineItemDto = new ProposalTemplateLineItemDto
				{
					Id = category.ID.Value,
					Name = category.Name,
					EstimateCategoryId = category.ID.Value,
					ParentId = null,
					Sequence = parentFromTemplate != null ? parentFromTemplate.Sequence : (category.Sequence == null ? 1 : category.Sequence.Value),
					LineItems = new List<ProposalTemplateLineItemDto>()
				};

				var dbLineItems = result.Where(pl => pl.ParentEstimateCategoryID == category.ID);
	
				foreach (var dbLineItem in dbLineItems)
				{
					var proposalLineDto = new ProposalLineItemDto();

					proposalLineDto.Sequence = dbLineItem.Sequence ?? 0;
					proposalLineDto.ItemName = dbLineItem.Name;
					proposalLineDto.CategoryId = dbLineItem.ParentEstimateCategoryID;
					proposalLineDto.ItemId = dbLineItem.EstimateCategoryID;
					proposalLineDto.Amount = dbLineItem.Amount;
					proposalLineDto.Description = dbLineItem.Description;
					proposalLineDto.Percentage = (decimal?)dbLineItem.Percentage;
					
			
					proposalTemplateLineItemDto.LineItems.Add(mapper.Map<ProposalTemplateLineItemDto>(proposalLineDto));
				}

				proposalTemplateLineItemDto.LineItems = proposalTemplateLineItemDto.LineItems.OrderBy(li => li.Sequence).ToList();
				retval.Add(proposalTemplateLineItemDto);
			}

			return retval;

		}
		private async Task<List<ProposalTemplateLineItemDto>> GetProposalLineItemCategoriesFromTemplateAsync(Guid proposalId)
		{
			var dbCategories = await ClientDbContext.ProposalLines
								.Where(pl => pl.ProposalId == proposalId)
								.Join(ClientDbContext.ProposalTemplatesLineItems,
									  pl => pl.ParentEstimateCategoryId,
									  pli => pli.Id,
									  (pl, pli) => new { pl.ParentEstimateCategoryId, pli.Name, pli.Sequence })
								.GroupBy(x => new { x.ParentEstimateCategoryId, x.Name, x.Sequence })
								.Select(g => new ProposalTemplateLineItemDto
								{
									Id = g.Key.ParentEstimateCategoryId.Value,
									Name = g.Key.Name,
									Sequence = g.Key.Sequence
								})
								.OrderBy(result => result.Sequence)
								.ToListAsync();

			var dbCategoryIdList = dbCategories.Select(c => c.Id).ToList();
			var dbProposalLines = ClientDbContext.ProposalLines
				.Where(p => p.ProposalId == proposalId && dbCategoryIdList.Contains(p.ParentEstimateCategoryId.Value))
				.ToList();

			var dbProposalLineIds = dbProposalLines.Select(p => p.EstimateCategoryId).ToList();
			var dbTemplateLines = ClientDbContext.ProposalTemplatesLineItems.Where(p => dbProposalLineIds.Contains(p.Id)).ToList();

			foreach (var category in dbCategories)
			{
				var dbLineItems = dbProposalLines.Where(pl => pl.ParentEstimateCategoryId == category.Id);
				category.LineItems = new List<ProposalTemplateLineItemDto>();

				foreach (var dbLineItem in dbLineItems)
				{
					var dbProposalTemplateLine = dbTemplateLines.FirstOrDefault(e => e.Id == dbLineItem.EstimateCategoryId);
					var proposalLineDto = mapper.Map<ProposalLineItemDto>(dbLineItem);

					if (dbProposalTemplateLine != null)
					{
						proposalLineDto.Sequence = dbProposalTemplateLine.Sequence;
						proposalLineDto.ItemName = dbProposalTemplateLine.Name;
					}

					category.LineItems.Add(mapper.Map<ProposalTemplateLineItemDto>(proposalLineDto));
				}

				category.LineItems = category.LineItems.OrderBy(li => li.Sequence).ToList();
			}

			return dbCategories;
		}
		private async Task<List<Proposal>> GetProposalsBySupervisorIdAsync(int? supervisorId, bool isArchived = false)
		{
			var docStatusFilter = new List<string> { "ACCEPTED", "DRAFT" };

			if (supervisorId.HasValue)
			{

				return await ClientDbContext.ProposalSupervisors
					.Where(ps => ps.UserId == supervisorId.Value)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.ProposalLines)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.Client)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.Qbclass)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.Template)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.ProposalSupervisors)
						.ThenInclude(p => p.User)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.CreatedByNavigation)
					.Include(ps => ps.Proposal)
						.ThenInclude(p => p.UpdatedByNavigation)
					.OrderByDescending(ps => ps.Proposal.DateCreated)
					.Where(ps=> docStatusFilter.Contains(ps.Proposal.DocStatus.ToUpper()) && ps.Proposal.IsArchived == isArchived)
					.Select(ps => ps.Proposal)
					.ToListAsync();
			}
			else
			{
				return await ClientDbContext.Proposals
					.Include(ps => ps.ProposalLines)
					.Include(ps=> ps.ProposalProject)
					.Include(ps => ps.Client)
					.Include(p => p.Qbclass)
					.Include(p => p.Template)
					.Include(ps => ps.ProposalSupervisors)
						.ThenInclude(p => p.User)
					.OrderByDescending(ps => ps.DateCreated)
					.Where(p => docStatusFilter.Contains(p.DocStatus.ToUpper()) && p.IsArchived == isArchived)
					.ToListAsync();
			}
		}
		private List<Proposal> FilterProposalsByStatus(List<Proposal> proposals, string status)
		{
			var statusfilter = status.ToLower();
			proposals = statusfilter switch
			{
				"accepted" => proposals.Where(p => p.DocStatus == "Accepted" && p.IsArchived == false).ToList(),
				"draft" => proposals.Where(p => p.DocStatus == "Draft" && p.IsArchived == false).ToList(),
				"archived" => proposals.Where(p => p.IsArchived == true).ToList(),
				_ => proposals,
			};

			return proposals;
		}
		private Proposal CreateProposal(ProposalDto proposalDto)
		{
			var proposal = new Proposal
			{
				Id = Guid.NewGuid(),
				TemplateId = proposalDto.Template.Id,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				Number = proposalDto.Number,
				Date = Convert.ToDateTime(proposalDto.Date),
				DocStatus = ((DocStatus)proposalDto.Status).GetStringValue(),
				TotalAmount = (decimal)proposalDto.Template.Categories.SelectMany(c=> c.LineItems).Sum(p => p.Amount),
				IsDeleted = false,
				//SqFeet = proposalDto.Project?.SqFeet ?? null,
			};

			ClientDbContext.Proposals.Add(proposal);
			return proposal;
		}
		private async Task PopulateProposalLinesAsync(ProposalModel proposalDto, Proposal proposal)
		{
			var proposalId = proposal.Id;
			var proposalLines = new List<ProposalLine>();
			var dbProposalLines = ClientDbContext.ProposalLines.Where(pl => pl.ProposalId == proposalId).ToList();
			ClientDbContext.RemoveRange(dbProposalLines);

			var parentCategories = proposalDto.Template.Categories.Select(c=> c.Name.ToLower().Trim()).Distinct().ToList();
			var dbParentCategories = await ClientDbContext.EstimateCategories
				.Where(ec => parentCategories.Contains(ec.Name.ToLower().Trim()) && ec.ParentEstimateCategoryId == null)
				.ToListAsync();

			foreach (var category in proposalDto.Template.Categories)
            {
				var parentCategory = dbParentCategories.FirstOrDefault(ec => ec.Name.ToLower().Trim() == category.Name.ToLower().Trim());

				if(parentCategory == null)
				{
					parentCategory = await AddCategory(category.Name, string.Empty, category.Sequence, null);
				}

				category.EstimateCategoryId = parentCategory.Id;

				var parentProposalLine = await AddParentProposalLine(proposalId, category);

				proposalLines.Add(parentProposalLine);

				var lineItems = category.LineItems;
				if(lineItems != null && lineItems.Any())
				{
					foreach (var lineItem in lineItems)
					{
						var newProposalLine = await MapProposalLineItemToEstimateCategory(proposalId, category, lineItem);
						proposalLines.Add(newProposalLine);
					}
				}

			}

			await ClientDbContext.ProposalLines.AddRangeAsync(proposalLines);
		}
		private async Task<ProposalLine> AddParentProposalLine(Guid proposalId, ProposalTemplateLineItemModel category)
		{
			return await Task.Run(() =>
			{
				ProposalLine proposalLine = new ProposalLine
				{
					Id = Guid.NewGuid(),
					Name = category.Name,
					ProposalId = proposalId,
					EstimateCategoryId = category.EstimateCategoryId.Value,
					Amount = 0,
					Sequence = category.Sequence,
					Updated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()
				};
				return proposalLine;
			});
		}
		private async Task<ProposalLine> MapProposalLineItemToEstimateCategory(Guid proposalId, ProposalTemplateLineItemModel category, ProposalTemplateLineItemModel proposalTemplateLineItemDto)
		{
			var newProposalLine = mapper.Map<ProposalLine>(proposalTemplateLineItemDto);
			var estimateCategory = ClientDbContext.EstimateCategories.Where(e => e.Name.ToLower() == newProposalLine.Name.ToLower()).FirstOrDefault();
			
			if (estimateCategory == null)
			{
				estimateCategory = await AddCategory(newProposalLine.Name, newProposalLine.Description, newProposalLine.Sequence.Value, category.EstimateCategoryId.Value);
			}

			newProposalLine.Id = Guid.NewGuid();
			newProposalLine.ProposalId = proposalId;
			newProposalLine.EstimateCategoryId = estimateCategory.Id;
			newProposalLine.ParentEstimateCategoryId = category.EstimateCategoryId.Value;
			newProposalLine.Sequence = proposalTemplateLineItemDto.Sequence;
			newProposalLine.Percentage = Convert.ToDouble(proposalTemplateLineItemDto.Percentage.Value);
			newProposalLine.SqFoot = proposalTemplateLineItemDto.SqFoot;
			newProposalLine.Multiplier = proposalTemplateLineItemDto.Multiplier;
			newProposalLine.SqFootLocked = proposalTemplateLineItemDto.SqFootLocked;
			newProposalLine.Updated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			return newProposalLine;
		}
		private async Task<EstimateCategory> AddCategory(string name, string description, int sequence, Guid? parentCategoryId)
		{
			var estimateCategory = new EstimateCategory
			{
				Id = Guid.NewGuid(),
				Created = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				Name = name,
				Sequence = sequence,
				ParentEstimateCategoryId = parentCategoryId,
				Description = description
			};

			await ClientDbContext.EstimateCategories.AddAsync(estimateCategory);
			return estimateCategory;
		}
		private async Task<EstimateCategory> AddParentCategory(string name, int sequence)
		{
			var parentEstimateCategory = new EstimateCategory
			{
				Id = Guid.NewGuid(),
				Created = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				Name = name,
				Sequence = sequence,
				ParentEstimateCategoryId = null
			};

			await ClientDbContext.EstimateCategories.AddAsync(parentEstimateCategory);

			return parentEstimateCategory;
		}
		private async Task PopulateSupervisorsAsync(ProposalDto proposalDto, Proposal proposal)
		{
			var currentProjectSupervisors = await ClientDbContext.ProjectSupervisors.Where(p => p.ProjectId == proposal.QbclassId).ToListAsync();
			var deletedSupervisors = currentProjectSupervisors?.Where(ci => !proposalDto.Supervisors.Any(ni => ni == ci.SupervisorId)).ToList();
			
			if (deletedSupervisors != null)
				ClientDbContext.RemoveRange(deletedSupervisors);

			foreach (var supervisorId in proposalDto.Supervisors)
			{
				if(currentProjectSupervisors ==null || !currentProjectSupervisors.Select(s=> s.SupervisorId).ToList().Contains(supervisorId))
				{
					var projectSupervisor = new ProjectSupervisor { ProjectId = proposal.QbclassId.Value, SupervisorId = supervisorId, SupervisorTypeId = 1 };
					ClientDbContext.ProjectSupervisors.Add(projectSupervisor);
				}
			}
		}
		private async Task PopulateProjectAsync(ProposalDto proposalDto, Proposal proposal, ApplicationUserShortDetailsDto defaultSupervisor)
		{
			var project = proposalDto.Project;

			Qbclass qbClass;
			if (project.Id == Guid.Empty)
			{
				var proposalGeneratedName = $"{proposalDto.Number}-{defaultSupervisor.FirstName}-{proposalDto.Date}";
				var projectName = project.Name == string.Empty ? proposalGeneratedName : project.Name;
				qbClass = new Qbclass
				{
					Id = Guid.NewGuid(),
					Name = projectName,
					FullyQualifiedName = projectName,
					Address = project.Address,
					ListId = "NonQBOProject",
					State = project.State,
					City = project.City,
					IsActive = true,
					Description = project.Description
				};
				ClientDbContext.Qbclasses.Add(qbClass);
			}
			else
			{
                qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(p => p.Id == project.Id);
                if (qbClass != null)
				{
					qbClass.Name = project.Name;
					qbClass.FullyQualifiedName = project.Name;
					qbClass.Address = project.Address;
					qbClass.City = project.City;
					qbClass.State = project.State;
					qbClass.Description = project.Description;
					qbClass.IsActive = true;
				}
			}

			proposal.QbclassId = qbClass.Id;
		}	
		private async Task<ApplicationUserShortDetailsDto> GetMainSupervisor(ProposalDto proposalDto)
		{
			var mainSupervisorId = proposalDto.Supervisors.FirstOrDefault();
			var user = await ClientDbContext.Users.FirstOrDefaultAsync(u=> u.Id == mainSupervisorId);
			return mapper.Map<ApplicationUserShortDetailsDto>(user);
		}
		private async Task AddClientUser(Proposal proposal, Guid projectId)
		{
			var proposalClient = proposal.Client;
			if (proposalClient == null) throw new Exception("Client not found.");

			int userId = 0;

			var existingUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Email == proposalClient.EmailAddress);
			if (existingUser == null)
			{
				var code = GenerateConfirmationCode();
				var user = new Core.ApiPayloadModels.UserPayload
				{
					Email = proposalClient.EmailAddress,
					FirstName = proposalClient.Name ?? "",
					LastName = "",
					Password = "Password@123",
					RoleId = (int)Roles.Client,
					ConfirmationCode = code,
					Status = (int)UserStatus.Pending
				};

				var result = await userService.CreateAsync<ApplicationUserDto>(user);

				if (result != null)
				{
					userId = result.Id;
					//send email invite

					var request = httpContextAccessor.HttpContext?.Request;
					var baseUrl = $"{request.Scheme}://{request.Host}";
					var link = $"{baseUrl}/confirm-client-account?code={code}";

                    var emailModel = new EmailPayloadModel
					{
						To = result.Email,
						Subject = $"You're Invited to ContractorsDesk - Activate Your Account:",
						Body = GenerateConfirmClientAccountEmailBody(link)
					};

					var emailSent = await emailService.SendEmailAsync(emailModel);
				}
			}
			else
			{
				userId = existingUser.Id;
			}

            //update client to link to user
            proposal.Client.UserId = userId;

            // populate client project
            var clientproject = new ClientProject { UserId = userId, ProjectId = projectId };
			ClientDbContext.ClientProjects.Add(clientproject);
		}
		private async Task<ProposalTemplateUserDefaultDto?> GetProposalTemplateUserDefault()
		{
			var proposalTemplate = await ClientDbContext.ProposalTemplateUserDefaults
				.FirstOrDefaultAsync(p => p.UserId == this.UserId);

			if (proposalTemplate != null)
			{
				return new ProposalTemplateUserDefaultDto
				{
					TemplateId = proposalTemplate.TemplateId,
					UserId = this.UserId
				};
			}

			return null;
		}
		private async Task<ProposalTemplate?> GetTemplate()
		{
			var userDefaultTemplate = await GetProposalTemplateUserDefault();

			Expression<Func<ProposalTemplate, bool>> predicate = userDefaultTemplate == null ? pt => pt.IsDefault : pt => pt.Id == userDefaultTemplate.TemplateId;
			return await ClientDbContext
				.ProposalTemplates
				.Include(pt=> pt.ProposalTemplatesLineItems)
				.AsNoTracking().FirstOrDefaultAsync(predicate);
		}
		private async Task<bool> EnsureUniqueProposalLinesAsync(Guid proposalId)
		{
			await this.ClientDbContext.Database.ExecuteSqlRawAsync("EXEC spEnsureUniqueProposalLines @ProposalID = {0}", proposalId);
			return true;
		}
        private string GenerateConfirmClientAccountEmailBody(string link)
        {
            var uri = new Uri(link);
            var siteDomainUrl = uri.Scheme + "://" + uri.Host;
            var emailBody = new StringBuilder();

            emailBody.Append("<html>");
            emailBody.Append("    <head>");
            emailBody.Append("        <style>html,body { padding: 0; margin:0; }</style>");
            emailBody.Append("    </head>");
            emailBody.Append("    <body>");
            emailBody.Append("        <div style=\"font-family:Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: "
                                    + "15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#181c32!important\">");
            emailBody.Append("            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"border-collapse:collapse;"
                                        + "margin:0 auto; padding:0; max-width:600px\">");
            emailBody.Append("                <tbody>");
            emailBody.Append("                    <tr>");
            emailBody.Append("                        <td align=\"center\" valign=\"center\" style=\"text-align:center; padding: 40px\">");
            emailBody.Append($"                            <a href=\"{siteDomainUrl}\" rel=\"noopener\" target=\"_blank\">");
            emailBody.Append($"                                <img alt=\"Logo\" src=\"{siteDomainUrl}/assets/media/logos/6.png\" style=\"width: 269px;\" />");
            emailBody.Append("                            </a>");
            emailBody.Append("                        </td>");
            emailBody.Append("                    </tr>");
            emailBody.Append("                    <tr>");
            emailBody.Append("                        <td align=\"left\" valign=\"center\">");
            emailBody.Append("                            <div style=\"text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px\">");
            emailBody.Append("                                <div style=\"padding-bottom: 30px; font-size: 17px;\">");
            emailBody.Append("                                    <strong>Hello!</strong>");
            emailBody.Append("                                </div>");
            emailBody.Append("                                <div style=\"padding-bottom: 30px\">An account has been created for you on ContractorsDesk, where you can easily manage your projects, documents, and communications.\r\n "
                                                            + "To proceed with the account confirmation, please click on the button below:</div>");
            emailBody.Append("                                <div style=\"padding-bottom: 40px; text-align:center;\">");
            emailBody.Append($"                                    <a href=\"{link}\" rel=\"noopener\" style=\"text-decoration:none;display:inline-block;"
                                                                + $"text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;"
                                                                + $"color:#ffffff;background-color:#009EF7;border:0px;margin-right:0.75rem!important;font-weight:600!important;"
                                                                + $"outline:none!important;vertical-align:middle\" target=\"_blank\">Confirm Account</a>");
            emailBody.Append("                                </div>");
            emailBody.Append("                                <div style=\"border-bottom: 1px solid #eeeeee; margin: 15px 0\"></div>");
            emailBody.Append("                                <div style=\"padding-bottom: 10px\">Kind regards,");
            emailBody.Append("                                <br>ContractorDesk.");
            emailBody.Append("                                <tr>");
            emailBody.Append("                                    <td align=\"center\" valign=\"center\" style=\"font-size: 13px; text-align:center;padding: 20px; color: #6d6e7c;\">");
            emailBody.Append("                                        <p>Copyright ©");
            emailBody.Append($"                                        <a href=\"{siteDomainUrl}\" rel=\"noopener\" target=\"_blank\">ContractorDesk</a>.</p>");
            emailBody.Append("                                    </td>");
            emailBody.Append("                            </div>");
            emailBody.Append("                        </td>");
            emailBody.Append("                    </tr>");
            emailBody.Append("                </tbody>");
            emailBody.Append("            </table>");
            emailBody.Append("        </div>");
            emailBody.Append("    </body>");
            emailBody.Append("</html>");

            return emailBody.ToString();
        }
        private static string GenerateConfirmationCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private async Task<bool> IsProposalNameDuplicateAsync(string name, Guid? excludeId = null)
        {
            var query = ClientDbContext.Qbclasses
                .Where(p => p.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
		public async Task<bool> ArchiveProposal(Guid id, bool isArchived = true)
		{
			var dbProposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == id);

			if (dbProposal == null) throw new ArgumentException($"Unable to find proposal with and Id of {id}");

			dbProposal.IsArchived = isArchived;
			await ClientDbContext.SaveChangesAsync();

			return true;
		}
		#endregion

	}
}

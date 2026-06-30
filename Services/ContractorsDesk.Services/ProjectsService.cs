using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ContractorsDesk.Services
{
	public class ProjectsService : BaseService, IProjectsService
	{
		private readonly ICacheService cacheService;
		private readonly IApplicationUserService userService;
		private readonly IPostMarkEmailService emailService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly Lazy<IEstimateService> estimateService;
		public ProjectsService(
			ICacheService cacheService,
			IApplicationUserService userService,
			IPostMarkEmailService emailService,
			Lazy<IEstimateService> estimateService,
			IHttpContextAccessor httpContextAccessor,
			ClientDbContext clientDataDbContext,
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration) :
			base(mapper, clientDataDbContext, masterDbContext, configuration)
		{
			this.userService = userService;
			this.emailService = emailService;
			this.estimateService = estimateService;
			this.cacheService = cacheService;
			this.httpContextAccessor = httpContextAccessor;
		}

		#region Public Methods
		public async Task<List<ProjectDetailsDto>> GetActiveJobsByUserAsync(Roles role, int userId, bool canManageAllProjects, string? searchKey = null)
		{
			var activeJobs = await ClientDbContext.VwSupervisorAndClientActiveJobs
				.Where(a=> !a.IsArchived)
				.OrderBy(v => v.Name)
				.ToListAsync();

			return await this.GetProjectDetails(activeJobs, canManageAllProjects, userId);
		}
		public async Task<List<ProjectDetailsDto>> GetArchivedJobsByUserAsync(Roles role, int userId, bool canManageAllProjects)
		{
			var activeJobs = await ClientDbContext.VwSupervisorAndClientActiveJobs
				.Where(a => a.IsArchived)
				.OrderBy(v => v.Name)
				.ToListAsync();

			return await this.GetProjectDetails(activeJobs, canManageAllProjects, userId);
		}
		public async Task<List<ProjectDto>> GetActiveProjectsAsync()
		{
			var projectList = new List<ProjectDto>();

			var activeJobs =  await ClientDbContext.VwActiveJobs
				.OrderBy(v => v.Name)
				.ToListAsync();

			var projectIds = activeJobs.Select(p => p.Id).ToList();

			// get the list of project supervisors in all active projects
			var dbProjectSupervisors = ClientDbContext.ProjectSupervisors
				.Where(ps => projectIds.Contains(ps.ProjectId))
				.ToList(); //projects.SelectMany(p => p.ProjectSupervisors).Select(p => p.SupervisorId).Distinct().ToList();

			var projectSupervisorIds = dbProjectSupervisors.Select(s => s.SupervisorId).Distinct().ToList();

			return await GetProjects(activeJobs, projectIds, projectSupervisorIds, dbProjectSupervisors);
		}
		public async Task<List<ProjectDto>> GetPendingProjectsAsync()
		{
			var proposalProjects = await ClientDbContext.Proposals
				.Include(pr=> pr.Qbclass)
				.Where(pr => pr.DocStatus == "Draft")
				.Select(pr => pr.Qbclass)
				.ToListAsync();

			return mapper.Map<List<ProjectDto>>(proposalProjects);
		}
		public async Task<List<ProjectDto>> GetProjectsBySupervisorIdAsync()
		{
			var projectList = new List<ProjectDto>();

			// get the list of project supervisors from the logged-in user
			var dbProjectSupervisors = ClientDbContext.ProjectSupervisors
				.Where(ps => ps.SupervisorId == this.UserId)
				.ToList();

			var projectSupervisorIds = dbProjectSupervisors.Select(s => s.SupervisorId).Distinct().ToList();
			var projectIds = dbProjectSupervisors.Select(p => p.ProjectId).ToList();

			var activeJobs = await ClientDbContext.VwActiveJobs
				.Where(a => projectIds.Contains(a.Id))
				.OrderBy(v => v.Name)
				.ToListAsync();

			return await GetProjects(activeJobs, projectIds, projectSupervisorIds, dbProjectSupervisors);
		}
		public async Task<List<ClientProjectDto>> GetClientProjects()
		{
			var retval = new List<ClientProjectDto>();
			var clientProjects = await this.ClientDbContext.ClientProjects
				.Include(cp => cp.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(p => p.ProposalProject)
				.Include(cp => cp.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(p => p.ProposalSupervisors)
					.ThenInclude(p => p.User)
				.Include(cp => cp.Project)
					.ThenInclude(p => p.ProjectSchedules)
					.ThenInclude(p => p.ProjectScheduleTasks)
				.Where(c => c.UserId == this.UserId)
				.ToListAsync();

			foreach (var clientProject in clientProjects)
			{
				List<ProjectScheduleTask>? scheduleTasks = null;

				var proposal = clientProject.Project.Proposals.FirstOrDefault();
				var proposalProject = proposal?.ProposalProject;
				var proposalSupervisors = proposal?.ProposalSupervisors;
				var schedule = clientProject.Project.ProjectSchedules.FirstOrDefault();

				if(schedule != null)
					scheduleTasks = schedule.ProjectScheduleTasks.ToList();

				var supervisors = proposal.ProposalSupervisors.Select(ps => ps.User).ToList();
				var address = string.Empty;
				var budget = 0;
				var budgetToActual = await estimateService.Value.GetEstimateToActualAsync(proposal.Id);

				var clientProjectDto = new ClientProjectDto();
				clientProjectDto.Name = proposalProject.Name;
				
				if(!string.IsNullOrEmpty(proposalProject.Address))
					address += proposalProject.Address;

				if (!string.IsNullOrEmpty(proposalProject.City))
					address += $",{proposalProject.City}";

				if (!string.IsNullOrEmpty(proposalProject.State))
					address += $",{proposalProject.State}";


				if(scheduleTasks != null)
				{
					var estimatedCompletionDate = scheduleTasks.OrderByDescending(t => t.EndDate).FirstOrDefault()?.EndDate;
					var startDate = scheduleTasks.OrderBy(t => t.StartDate).FirstOrDefault()?.StartDate;

					if (estimatedCompletionDate != null)
						clientProjectDto.EstimatedCompletionDate = estimatedCompletionDate.Value;

					if (startDate != null)
						clientProjectDto.StartDate = startDate.Value;
				}

				if(supervisors.Any())
				{
					clientProjectDto.AssignedSupervisors = mapper.Map<List<ApplicationUserShortDetailsDto>>(supervisors);
				}

				if(budgetToActual != null)
				{
					clientProjectDto.Budget = budgetToActual.EstimateCategories?.Sum(e => e.TotalRevised);
				}

				clientProjectDto.Id = clientProject.Project.Id;
				clientProjectDto.Address = address;
				clientProjectDto.Status = clientProject.Project.IsActive == true ? "In Progress" : "Archived";

				retval.Add(clientProjectDto);
			}

			return retval;
		}
		public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
		{
			ProjectDto? retval = null;
			
				var project = await this.ClientDbContext.Qbclasses
					.Where(c => c.Id == id)
					.Include(c => c.Qbaccount)
					.Include(c => c.ProjectNotes)
						.ThenInclude(pn=> pn.CreatedByNavigation)
                    .Include(c => c.ProjectNotes)
                        .ThenInclude(pn => pn.UpdatedByNavigation)
                    .Include(p => p.Proposals)
						.ThenInclude(pr => pr.Client)
					.Include(p => p.Proposals)
						.ThenInclude(pr => pr.ProposalSupervisors)
						.ThenInclude(ps => ps.User)
					.Include(p => p.Proposals)
						.ThenInclude(pr => pr.ProposalProject)

					.FirstOrDefaultAsync() ?? throw new ArgumentNullException($"Cannot find project with an ID of {id}");

				var docStatus = DocStatus.Accepted.GetStringValue();
				retval = mapper.Map<ProjectDto>(project);
				
				var proposal = project.Proposals.FirstOrDefault(p=> p.IsDeleted != true);

				if (proposal != null)
				{
					//get the customer details
					var client = proposal.Client;
					var supervisors = proposal.ProposalSupervisors;
					var proposalProject = proposal.ProposalProject;

					if (client != null)
					{
						retval.Client = mapper.Map<ClientDto>(client);
					}

					if (supervisors.Any())
					{
						foreach (var supervisor in supervisors)
						{
							var userDto = mapper.Map<ApplicationUserDto>(supervisor.User);
							userDto.SupervisorTypeId = 1; //supervisor.SupervisorTypeId;
							retval.Supervisors.Add(userDto);
						}
					}

					if(proposalProject != null) {
						retval.Description = proposalProject.Description;
						retval.Name = proposalProject.Name;
						retval.FullyQualifiedName = proposalProject.Name;
					}

					retval.ProposalId = proposal.Id;
					DocStatus statusEnum = EnumExtensions.GetEnumValueFromString<DocStatus>(proposal.DocStatus);

					var proposalDto = new ProposalDto();
					proposalDto.Id = proposal.Id;
					proposalDto.Number = proposal.Number;
					proposalDto.Status = (int)statusEnum;
					retval.Proposal = proposalDto;
					retval.IsAccepted = proposalDto.Status == (int)DocStatus.Accepted;

				}

				var projectSchedule = await ClientDbContext.ProjectSchedules.AsNoTracking().FirstOrDefaultAsync(ps => ps.ProjectId == id);

				var transaction = await ClientDbContext.Qbtransactions
				.Where(t => t.ClassId == id)
				.OrderBy(t => t.TransactionDate)
				.FirstOrDefaultAsync();

				retval.HasSchedule = projectSchedule != null;
				retval.HasQBTransactions = transaction != null;
				retval.ProjectNote = mapper.Map<ProjectNoteDto>(project.ProjectNotes.FirstOrDefault());

			return retval;
		}
		public async Task<ProjectDetailsViewDto> GetProjectDetailsByIdAsync(Guid id)
		{
			var retval = new ProjectDetailsViewDto();

			var dbQbClass = await ClientDbContext.Qbclasses
				.Include(q => q.Proposals)
					.ThenInclude(p => p.Client)
				.Include(q => q.Proposals)
					.ThenInclude(p => p.ProposalProject)
				.Include(q => q.Proposals)
					.ThenInclude(p => p.ProposalSupervisors)
					.ThenInclude(p=> p.User)
				.Include(q => q.ProjectSchedules)
					.ThenInclude(p => p.ProjectScheduleTasks)
				.Include(q => q.ProjectNotes)
				.FirstOrDefaultAsync(q => q.Id == id)
				?? throw new Exception($"Cannot find project with an ID of {id}");

			var dbProposal = dbQbClass.Proposals.FirstOrDefault();
			var dbClient = dbProposal?.Client;
			var dbProposalProject = dbProposal?.ProposalProject;
			var dbProjectNotes = dbQbClass.ProjectNotes.FirstOrDefault();
			var dbSupervisors = dbProposal?.ProposalSupervisors.ToList();
			var projectManagers = dbSupervisors.Where(s => s.SupervisorTypeId == (int)SupervisorType.ProjectManager).ToList();
			var asstProjectManagers = dbSupervisors.Where(s => s.SupervisorTypeId == (int)SupervisorType.AsstProjectManager).ToList();
			var onsiteSupervisors = dbSupervisors.Where(s => s.SupervisorTypeId == (int)SupervisorType.OnsiteSupervisor).ToList();
			var schedule = dbQbClass.ProjectSchedules.FirstOrDefault();

			retval.Id = dbQbClass.Id;
			retval.ProposalId = dbProposal.Id;
			retval.Name = dbProposalProject.Name;
			retval.Notes = dbProjectNotes?.Notes;
			retval.IsActive = dbQbClass.IsActive;
			retval.IsArchived = dbQbClass.IsArchived;
			retval.IsSpecJob = dbQbClass.ActiveSpecJobs;
			retval.PhotoUrl = dbProposalProject.PhotoUrl ?? "/assets/media/logos/project-logo.png";
			retval.ProjectManagers = mapper.Map<List<ApplicationUserShortDetailsDto>>(projectManagers.Select(pm => pm.User).ToList());
			retval.AssistantProjectManagers = mapper.Map<List<ApplicationUserShortDetailsDto>>(asstProjectManagers.Select(pm => pm.User).ToList());
			retval.OnsiteSupervisors = mapper.Map<List<ApplicationUserShortDetailsDto>>(onsiteSupervisors.Select(pm => pm.User).ToList());
			retval.ClientDetails = mapper.Map<ClientDto>(dbClient);
			retval.ProjectDetails = mapper.Map<ProposalProjectDto>(dbProposalProject);
			
			if(schedule != null)
			{
				retval.HasSchedule = true;
				retval.StartDate = schedule.ProjectScheduleTasks.OrderBy(t => t.StartDate).FirstOrDefault()?.StartDate;
				retval.EndDate = schedule.ProjectScheduleTasks.OrderByDescending(t => t.EndDate).FirstOrDefault()?.EndDate;
			}

            var budgetToActual = await estimateService.Value.GetEstimateToActualAsync(dbProposal.Id);
			if(budgetToActual != null)
			{
				retval.Budget = budgetToActual.EstimateCategories?.Sum(e => e.TotalRevised);
            }

            return retval;
		}
		public async Task<ProjectDetailsDto> GetProjectShortDetailsAsync(Guid id)
		{
			ProjectDetailsDto? retval = new ProjectDetailsDto();

			var projectSupervisors = await ClientDbContext.VwProjectShortDetails.Where(p => p.Id == id).ToListAsync();
			if (projectSupervisors == null || !projectSupervisors.Any()) throw new ArgumentNullException($"Cannot find project with an ID of {id}");
			
			var projectSchedule = await ClientDbContext.ProjectSchedules.AsNoTracking().FirstOrDefaultAsync(ps => ps.ProjectId == id);

			var project = projectSupervisors.FirstOrDefault();	
			retval = new ProjectDetailsDto
			{
				Id = project.Id,
				Name = project.Name,
				FullyQualifiedName = project.FullyQualifiedName,
				JobBalance = project.JobBalance,
				IsArchived = project.IsArchived,
				ProposalId = project.ProposalId,
				ClientEmailAddress = project.ClientEmailAddress,
				ClientName = project.ClientName,
				IsAccepted = project.DocStatus == "Accepted",
                StartDate = project.StartDate.HasValue ? project.StartDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                EndDate = project.EndDate.HasValue ? project.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty,
				IsActive = project.IsActive,
				HasSchedule = projectSchedule != null
			};

			foreach (var supervisor in projectSupervisors)
			{
				if(supervisor.SupervisorId != null)
				{
					var userDto = new ApplicationUserShortDetailsDto
					{
						Id = supervisor.SupervisorId.Value,
						FirstName = supervisor.SupervisorFirstName,
						LastName = supervisor.SupervisorLastName,
						Email = supervisor.SupervisorEmail
					};
					retval.Supervisors.Add(userDto);
				}
			}

			return retval;
		}
		public async Task<ClientDto> UpdateClientDetailsAsync(Guid projectId, ClientModel clientModel)
		{
			var project = await ClientDbContext.Qbclasses
				.Include(q => q.Proposals)
					.ThenInclude(p => p.Client)
				.FirstOrDefaultAsync(p => p.Id == projectId)	
				?? throw new ArgumentNullException($"Cannot find project with an ID of {projectId}");

			var proposal = project.Proposals.FirstOrDefault()
				?? throw new ArgumentNullException($"Cannot find proposal record for {project.Name}");

			var proposalClient = proposal.Client;
			var existingEmailAddressError = "Unable to save client details. A client with the same email address already exists.";

			int userId = 0;

			if (clientModel.Id != null)
			{
				var existingClient = ClientDbContext.Clients
					.Where(c => c.Id != clientModel.Id.Value && c.EmailAddress.ToLower().Trim() == clientModel.EmailAddress.ToLower().Trim())
					.AsNoTracking().FirstOrDefault();

				if (existingClient != null)
					throw new Exception(existingEmailAddressError);

				var dbClient = await ClientDbContext.Clients
					.FirstOrDefaultAsync(c => c.Id == clientModel.Id.Value);

				mapper.Map(clientModel, dbClient);
				proposal.ClientId = dbClient.Id;
				userId = dbClient.UserId ?? 0;
			}
			else
			{
				var existingClient = ClientDbContext.Clients
					.Where(c => c.EmailAddress.ToLower().Trim() == clientModel.EmailAddress.ToLower().Trim())
					.AsNoTracking().FirstOrDefault();

				if (existingClient != null)
					throw new Exception(existingEmailAddressError);

				var newClient = mapper.Map<Client>(clientModel);
				newClient.Id = Guid.NewGuid();
				proposal.ClientId = newClient.Id;
				ClientDbContext.Clients.Add(newClient);

				//send invite here
				userId = await AddClientUser(newClient, projectId);

			}

			//update client projects
			var clientProject = await ClientDbContext.ClientProjects.FirstOrDefaultAsync(cp => cp.ProjectId == projectId);
			if (clientProject == null)
			{
				var clientproject = new ClientProject { UserId = userId, ProjectId = projectId };
				ClientDbContext.ClientProjects.Add(clientproject);
			}
			else
			{
				clientProject.UserId = userId;
			}

			await ClientDbContext.SaveChangesAsync();
			return mapper.Map<ClientDto>(proposal.Client);
		}
		public async override Task<T> UpdateAsync<T>(object param)
		{
			if (param is ProjectPayload project)
			{
				var dbProject = await this.ClientDbContext.Qbclasses.FirstOrDefaultAsync(p=> p.Id == project.Id)
					?? throw new ArgumentNullException($"Cannot find project with an ID of {project.Id}");

				var proposal = await ClientDbContext.Proposals
					.Include(p=> p.ProposalProject)
					.FirstOrDefaultAsync(p => p.QbclassId == dbProject.Id)
					?? throw new ArgumentNullException($"Cannot find proposal record for {dbProject.Name}");

				//if (await IsProposalNameDuplicateAsync(project.Name, project.Id))
				//    throw new InvalidOperationException("A project with this name already exists.");

				var proposalId = proposal.Id;
				var proposalSupervisors = ClientDbContext.ProposalSupervisors.Where(s => s.ProposalId == proposalId);
				ClientDbContext.ProposalSupervisors.RemoveRange(proposalSupervisors);

				var projectSupervisors = project.Supervisors;
				if (projectSupervisors != null && projectSupervisors.Any())
				{
					ClientDbContext.ProposalSupervisors.AddRange(
					projectSupervisors.Select(su => new ProposalSupervisor
					{
						Id = Guid.NewGuid(),
						ProposalId = proposalId,
						UserId = su.Id,
						SupervisorTypeId = su.SupervisorTypeId
					}));
				}

				var isSpecJob = project.JobTypeId == 2;
				var isActive = project.StatusId == 2;

				dbProject.Name = project.Name;
				dbProject.FullyQualifiedName = project.Name;
				dbProject.ActiveJobs = !isSpecJob;
				dbProject.ActiveSpecJobs = isSpecJob;
				dbProject.IsActive = isActive;
				dbProject.IsArchived = !isActive;

				if(proposal.ProposalProject != null)
				{
                    proposal.ProposalProject.Name = project.Name;
					proposal.ProposalProject.Address = project.Address ?? string.Empty;
					proposal.ProposalProject.City = project.City ?? string.Empty;
					proposal.ProposalProject.State = project.State ?? string.Empty;
				}

                await ClientDbContext.SaveChangesAsync();

				var projectDto = await this.GetProjectByIdAsync(dbProject.Id);
				return mapper.Map<T>(projectDto);
			}
			else
			{
				throw new ArgumentException("Invalid payload type for UpdateAsync");
			}
		}
		public async Task<ProposalDto> GetProjectProposal(Guid projectId)
		{
			var proposal = await ClientDbContext.Proposals
				.Include(p=> p.Client)
				.FirstOrDefaultAsync(p => p.QbclassId == projectId);

			if (proposal == null) throw new Exception("Proposal not found.");

			var proposalDto = mapper.Map<ProposalDto>(proposal);
			DocStatus statusEnum = EnumExtensions.GetEnumValueFromString<DocStatus>(proposal.DocStatus);

			proposalDto.Client = mapper.Map<ClientDto>(proposal.Client);
			proposalDto.Status = (int)statusEnum;
			return proposalDto;
		}
		public async Task<ProjectDto> GetProjectByProposalIdAsync(Guid proposalId)
		{
			var project = await ClientDbContext.Proposals
				.Include(p=> p.Qbclass)
				.Where(p => p.Id == proposalId)
				.Select(p => p.Qbclass)
				.FirstOrDefaultAsync();

			var docStatus = DocStatus.Accepted.GetStringValue();
			var projectDto = mapper.Map<ProjectDto>(project);
			return projectDto;
		}
		public async Task<ProjectDocumentDto> AddProjectDocumentAsync(ProjectDocumentDto projectDocumentDto)
		{
			var projectDocument = mapper.Map<ProjectDocument>(projectDocumentDto);
			ClientDbContext.ProjectDocuments.Add(projectDocument);
			await ClientDbContext.SaveChangesAsync();
			return projectDocumentDto;
		}
		public async Task<List<ProjectSupervisorDto>> GetProjectSupervisorsAsync(Guid projectId)
		{
            var supervisors = await ClientDbContext.Users.ToListAsync();
            var projectSupervisorDtos = supervisors
                .Select(s => new ProjectSupervisorDto
                {
                    SupervisorId = s.Id,
                    Supervisor = mapper.Map<ApplicationUserShortDetailsDto>(s)
                })
                .ToList();


            return projectSupervisorDtos;
		}
		public async Task ArchiveProjectAsync(Guid id)
		{
			var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(q => q.Id == id);
			if (qbClass == null) throw new ArgumentNullException($"Cannot find project with an ID of {id}");

			qbClass.IsArchived = true;
			qbClass.IsActive = false;

            var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == id);
            if (proposal != null)
            {
                proposal.IsArchived = true;
            }

            await ClientDbContext.SaveChangesAsync();
		}
		public async Task DeleteProjectAsync(Guid id)
		{
			var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(q => q.Id == id);
			if (qbClass == null) throw new ArgumentNullException($"Cannot find project with an ID of {id}");

			qbClass.IsDeleted = true;
			await ClientDbContext.SaveChangesAsync();
		}
		public async Task<ProjectEstimateCategoriesAndScheduleItemsDto> GetProjectEstimateCategoriesAndScheduleItemsAsync(Guid projectId)
		{
			var retval = new ProjectEstimateCategoriesAndScheduleItemsDto();

			var estimateCategories = await ClientDbContext.ProjectEstimateCategoriesSpResult
				.FromSqlRaw($"EXEC spCDGetEstimateCategoriesByProject '{projectId}'")
				.ToListAsync();

			var scheduleItems = await ClientDbContext.ProjectScheduleItemsSpResult
				.FromSqlRaw($"EXEC spCDGetProjectScheduleItems '{projectId}'")
				.ToListAsync();

			retval.EstimateCategories = estimateCategories.Select(r => new EstimateCategoryItemDto
			{
				EstimateCategoryID = r.EstimateCategoryID,
				Name = r.Name,
				CurrentAmount = r.CurrentAmount
			}).ToList();

			retval.ScheduleTasks = scheduleItems.Select(r => new ProjectScheduleTaskItemDto
			{
				Id = r.Id,
				Name = r.Name
			}).ToList();

			return retval;
		}
		public async Task<SubContractorDto> AddSubcontractorAsync(SubContractorPayloadModel model)
		{
			SubContractor? dbSubContractor = null;

            if (!string.IsNullOrWhiteSpace(model.Category))
            {
                var existingCategory = await ClientDbContext.SubContractorCategories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == model.Category.ToLower());

                if (existingCategory == null)
                {
                    var newCategory = new SubContractorCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Category,
                        IsActive = true,
                    };

                    ClientDbContext.SubContractorCategories.Add(newCategory);
                }
            }

            //update the subcontractor fields
            if (!model.IsNew)
			{
				dbSubContractor = await ClientDbContext.SubContractors
					.FirstOrDefaultAsync(s => s.Id == model.Id);

				if (dbSubContractor != null)
				{
					mapper.Map(model, dbSubContractor);
				}
			}
			else
			{
				dbSubContractor = mapper.Map<SubContractor>(model);
				dbSubContractor.Id = Guid.NewGuid();
				ClientDbContext.SubContractors.Add(dbSubContractor);
			}

			if(model.ProjectId != null && model.ProjectId != Guid.Empty)
			{
				var projectSubContractor = new ProjectSubContractor();
				projectSubContractor.Id = Guid.NewGuid();
				projectSubContractor.ProjectId = model.ProjectId.Value;
				projectSubContractor.SubContractorId = dbSubContractor.Id;

				ClientDbContext.ProjectSubContractors.Add(projectSubContractor);
			}
			
			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<SubContractorDto>(dbSubContractor); ;
		}
        public async Task UnArchiveProjectAsync(Guid id)
        {
            var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(q => q.Id == id);
            if (qbClass == null) throw new ArgumentNullException($"Cannot find project with an ID of {id}");

            qbClass.IsArchived = false;
			qbClass.IsActive = true;


            var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == id);
            if (proposal != null)
            {
                proposal.IsArchived = false;
            }

            await ClientDbContext.SaveChangesAsync();
        }
		public async Task<ProjectNoteDto> SaveProjectNote(ProjectNoteModel model)
		{
			ProjectNote? projectNote = mapper.Map<ProjectNote>(model);

			var dbProjectNote = await ClientDbContext.ProjectNotes.FirstOrDefaultAsync(pn => pn.ProjectId == model.ProjectId);

			if(dbProjectNote != null)
			{
				mapper.Map(model, dbProjectNote);
			}
			else
			{
				projectNote.Id = Guid.NewGuid();
				ClientDbContext.ProjectNotes.Add(projectNote);
            }

			await ClientDbContext.SaveChangesAsync();
			return mapper.Map<ProjectNoteDto>(projectNote);
		}

		public async Task UpdateProjectPhotoUrl(Guid projectId, string photoUrl)
		{
			var dbProject = await this.ClientDbContext.Qbclasses.FirstOrDefaultAsync(p => p.Id == projectId)
					?? throw new ArgumentNullException($"Cannot find project with an ID of {projectId}");

			var proposal = await ClientDbContext.Proposals
				.Include(p => p.ProposalProject)
				.FirstOrDefaultAsync(p => p.QbclassId == dbProject.Id)
				?? throw new ArgumentNullException($"Cannot find proposal record for {dbProject.Name}");

			if (proposal.ProposalProject != null)
			{
				proposal.ProposalProject.PhotoUrl = photoUrl;
			}

			await ClientDbContext.SaveChangesAsync();
		}

		public async Task<List<ActiveProjectDto>> GetSupervisorActiveJobs(Roles role, int supervisorId, bool canManageAllProjects)
		{
			var activeJobsList = new List<ActiveProjectDto>();

			var activeJobs = await ClientDbContext.VwSupervisorAndClientActiveJobs
				.Where(p=> p.IsArchived != true)
				.OrderBy(v => v.Name)
				.ToListAsync();

			var supervisorJobs = activeJobs.Select(ac => new { ac.SupervisorId, ac.SupervisorFirstName, ac.SupervisorLastName, JobId = ac.Id });

			if (!canManageAllProjects)
			{
				activeJobs = activeJobs
					.Where(ac => ac.SupervisorId == supervisorId)
					.ToList();
			}

			activeJobsList = activeJobs
								.Select(aj => new ActiveProjectDto 
								{
									Id = aj.Id,
									Name = aj.Name
								})
							.DistinctBy(p => p.Id)
							.ToList();

			return activeJobsList;
		}

		#endregion

		#region Private Methods

		private async Task<List<ProjectDetailsDto>> GetProjectDetails(List<VwSupervisorAndClientActiveJob> activeJobs, bool canManageAllProjects, int userId)
		{
			var retval = new List<ProjectDetailsDto>();
			var supervisorJobs = activeJobs.Select(ac => new { ac.SupervisorId, ac.SupervisorFirstName, ac.SupervisorLastName, JobId = ac.Id });

			if (!canManageAllProjects)
			{
				activeJobs = activeJobs
					.Where(ac => ac.SupervisorId == userId)
					.ToList();
			}

			retval = activeJobs
					.Select(aj => new ProjectDetailsDto
					{
						Id = aj.Id,
						ClientEmailAddress = aj.ClientEmailAddress,
						ClientName = aj.ClientFullName,
						ProposalId = aj.ProposalId,
						Name = aj.Name,
						JobBalance = aj.JobBalance,
						Threshold = aj.Threshold,
						IsArchived = aj.IsArchived,
						PhotoUrl = aj.PhotoUrl ?? "assets/media/logos/project-logo.png",
						IsAccepted = aj.DocStatus == DocStatus.Accepted.GetStringValue(),
					})
					.DistinctBy(p => p.Id)
					.ToList();

			foreach (var project in retval)
			{
				var supervisors = supervisorJobs.Where(sj => sj.JobId == project.Id).DistinctBy(s => s.SupervisorId).ToList();
				if (supervisors.Any() && supervisors.Count() > 0)
				{
					project.Supervisors = new List<ApplicationUserShortDetailsDto>();
					foreach (var subpervisor in supervisors)
					{
						if (subpervisor.SupervisorId != null && subpervisor.SupervisorFirstName != null && subpervisor.SupervisorLastName != null)
						{
							var supervisorDto = new ApplicationUserShortDetailsDto
							{
								Id = subpervisor.SupervisorId.Value,
								FirstName = subpervisor.SupervisorFirstName,
								LastName = subpervisor.SupervisorLastName,
							};
							project.Supervisors.Add(supervisorDto);
						}
					}
				}
			}

			return retval.OrderBy(p => p.GroupStatusId).ThenByDescending(p => p.JobBalance).ToList();
		}

		private async Task<int> AddClientUser(Client client, Guid projectId)
		{
			int userId = 0;

			var existingUser = await ClientDbContext.Users.FirstOrDefaultAsync(u => u.Email == client.EmailAddress);
			if (existingUser == null)
			{
				var code = GenerateConfirmationCode();
				var user = new Core.ApiPayloadModels.UserPayload
				{
					Email = client.EmailAddress,
					FirstName = client.Name ?? "",
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
			return userId;
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
		private async Task<List<ProjectDto>> GetProjects(List<VwActiveJob> activeJobs, List<Guid> projectIds, List<int> projectSupervisorIds, List<ProjectSupervisor> dbProjectSupervisors)
		{
			var projectList = new List<ProjectDto>();
			var proposals = await this.ClientDbContext.Proposals.Where(p => projectIds.Contains(p.QbclassId.Value)).ToListAsync();
			var qbClasses = await ClientDbContext.Qbclasses.Where(q => projectIds.Contains(q.Id)).ToListAsync();
			var supervisors = await ClientDbContext.Users.Where(u => projectSupervisorIds.Contains(u.Id)).ToListAsync();

			foreach (var project in activeJobs)
			{
				var qbClass = qbClasses.FirstOrDefault(q=> q.Id == project.Id);
				var projectDto = new ProjectDto();//mapper.Map<ProjectDto>(project);
				projectDto.Name = project.Name;
				projectDto.JobBalance = project.JobBalance;
				projectDto.Id = project.Id;
				projectDto.IsArchived = qbClass.IsArchived;
				projectDto.IsDeleted = qbClass.IsDeleted;
				projectDto.Supervisors = new List<ApplicationUserDto>();

				var projectSupervisors = dbProjectSupervisors.Where(dbp => dbp.ProjectId == project.Id).ToList();
				foreach (var projectSupervisor in projectSupervisors)
				{
					var user = supervisors.FirstOrDefault(u => u.Id == projectSupervisor.SupervisorId);

					if (user != null)
						projectDto.Supervisors.Add(mapper.Map<ApplicationUserDto>(user));
				}

				projectDto.ProposalId = proposals.FirstOrDefault(p => p.QbclassId == project.Id)?.Id;
				projectList.Add(projectDto);
			}

			return projectList;
		}
		private (string Start, string End) GetDateRange(DateTime? minimumStartDate = null)
		{
			var dateFormat = "MM/dd/yyyy";
			var minStartDate = minimumStartDate != null ? minimumStartDate.Value.ToString(dateFormat) : configuration["Defaults:MinimumStartDate"];
			var endDate = DateTime.UtcNow.ToString(dateFormat);
			return (Start: minStartDate, End: endDate);
		}

        #endregion
    }
}

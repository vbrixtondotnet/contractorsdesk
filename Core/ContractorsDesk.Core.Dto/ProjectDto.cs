using Newtonsoft.Json;
namespace ContractorsDesk.Core.Dto
{
	public class ProjectDto
	{
		public Guid Id { get; set; }
		public Guid? ProposalId { get; set; }
		public string? ListId { get; set; }
		public string? Name { get; set; }
		public string? FullyQualifiedName { get; set; }
		public bool? SubClass { get; set; }
		public string? ParentId { get; set; }
		public DateTime? TimeCreated { get; set; }
		public DateTime? TimeModified { get; set; }
		public bool? AllowedForJobReports { get; set; }
		public bool? AllowedForBudgetReports { get; set; }
		public bool? ActiveJobs { get; set; }
		public bool? ActiveSpecJobs { get; set; }		
		public bool? HasSchedule { get; set; }
		public string? CreatedBy { get; set; }
		public string? UpdatedBy { get; set; }
		public string? Description { get; set; }
		public string ClosedDateFormatted
		{
			get
			{
				return this.ClosedDate.HasValue ? this.ClosedDate?.ToShortDateString() : string.Empty;
			}
		}
		public string OpenedDateFormatted
		{
			get
			{
				return this.OpenedDate.HasValue ? this.OpenedDate?.ToShortDateString() : string.Empty;
			}
		}
		public string? Notes { get; set; }
		public bool? OpenJob { get; set; }
		public Guid? QbaccountId { get; set; }

		[JsonIgnore]
		public DateTime? OpenedDate { get; set; }
		[JsonIgnore]
		public DateTime? ClosedDate { get; set; }
		public string? Ownership { get; set; }
		public decimal? JobBalance { get; set; }
		public bool IsArchived { get; set; }
		public bool IsDeleted { get; set; }
		public virtual AccountDto? Account { get; set; }
		public virtual List<ApplicationUserDto> Supervisors { get; set; } = new List<ApplicationUserDto>();
		public ClientDto Client { get; set; }
		public ProposalDto Proposal { get; set; }
        public bool? HasQBTransactions { get; set; }
		public bool IsAccepted { get; set; } = false;
		public bool? IsActive { get; set; }
		public ProjectNoteDto? ProjectNote { get; set; }
	}
	public class ProjectDetailsDto
	{
		public Guid Id { get; set; }
		public Guid? ProposalId { get; set; }
		public string? Name { get; set; }
		public string? Address { get; set; }
		public string? ClientEmailAddress { get; set; }
		public string? ClientName { get; set; }
		public string? FullyQualifiedName { get; set; }
		public string? StartDate { get; set; }
		public string? EndDate { get; set; }
		public decimal? JobBalance { get; set; }
		public decimal? Threshold { get; set; }
		public bool IsArchived { get; set; }
		public string Status {
			get
			{
				return this.IsArchived ? "archived" : "active";
			}
		}
		public bool IsAccepted { get; set; } = false;
		public virtual List<ApplicationUserShortDetailsDto> Supervisors { get; set; } = new List<ApplicationUserShortDetailsDto>();
		public bool? IsActive { get; set; }
		public bool HasSchedule { get; set; }

		public string PhotoUrl { get; set; }

		public int GroupStatusId { get {
				var retval = 1;
				
				if(JobBalance < 0)
				{
					retval = 3;
				}
				else if (JobBalance < Threshold)
				{
					retval = 2;
				}
				return retval;
			} 
		}

    }
	public class ActiveProjectDto
	{
		public Guid Id { get; set; }
		public string? Name { get; set; }
	}
	public class ProjectDetailsViewDto
	{
		public Guid Id { get; set; }
		public Guid? ProposalId { get; set; }
		public string? Name { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public bool IsArchived { get; set; }
		public bool? IsActive { get; set; }
		public bool? IsSpecJob { get; set; }
		public bool HasSchedule { get; set; }
		public string Notes { get; set; }
		public string PhotoUrl { get; set; }
		public decimal? Budget { get;set; }
		public ClientDto? ClientDetails { get; set; } = new ClientDto();
		public ProposalProjectDto? ProjectDetails { get; set; } = new ProposalProjectDto();
		public virtual List<ApplicationUserShortDetailsDto> ProjectManagers { get; set; } = new List<ApplicationUserShortDetailsDto>();
		public virtual List<ApplicationUserShortDetailsDto> AssistantProjectManagers { get; set; } = new List<ApplicationUserShortDetailsDto>();
		public virtual List<ApplicationUserShortDetailsDto> OnsiteSupervisors { get; set; } = new List<ApplicationUserShortDetailsDto>();
	}
}
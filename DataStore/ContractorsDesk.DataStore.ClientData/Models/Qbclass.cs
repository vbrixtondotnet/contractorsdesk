using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qbclass
{
    public Guid Id { get; set; }

    public string? ListId { get; set; }

    public string? Name { get; set; }

    public string? FullyQualifiedName { get; set; }

    public string? Address { get; set; }

    public bool? SubClass { get; set; }

    public string? ParentId { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? AllowedForJobReports { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string? Notes { get; set; }

    public bool? OpenJob { get; set; }

    public Guid? QbaccountId { get; set; }

    public DateTime? OpenedDate { get; set; }

    public string? Ownership { get; set; }

    public bool? ActiveJobs { get; set; }

    public bool? ActiveSpecJobs { get; set; }

    public bool? AllowedForBudgetReports { get; set; }

    public string? Description { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public bool IsArchived { get; set; }

    public bool IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public decimal? MinimumRequestedAmount { get; set; }

    public bool IsCompleted { get; set; }

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();

    public virtual ICollection<AudioUpload> AudioUploads { get; set; } = new List<AudioUpload>();

    public virtual ICollection<ClientProject> ClientProjects { get; set; } = new List<ClientProject>();

    public virtual ICollection<ConstructionTask> ConstructionTasks { get; set; } = new List<ConstructionTask>();

    public virtual ICollection<CostRevision> CostRevisions { get; set; } = new List<CostRevision>();

    public virtual ICollection<EstimateHistory> EstimateHistories { get; set; } = new List<EstimateHistory>();

    public virtual ICollection<EstimateMapping> EstimateMappings { get; set; } = new List<EstimateMapping>();

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<JobBalance> JobBalances { get; set; } = new List<JobBalance>();

    public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();

    public virtual ICollection<ProjectJournal> ProjectJournals { get; set; } = new List<ProjectJournal>();

    public virtual ICollection<ProjectNote> ProjectNotes { get; set; } = new List<ProjectNote>();

    public virtual ICollection<ProjectSchedule> ProjectSchedules { get; set; } = new List<ProjectSchedule>();

    public virtual ICollection<ProjectSubContractor> ProjectSubContractors { get; set; } = new List<ProjectSubContractor>();

    public virtual ICollection<ProjectSupervisor> ProjectSupervisors { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ProjectThreshold> ProjectThresholds { get; set; } = new List<ProjectThreshold>();

    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();

    public virtual Qbaccount? Qbaccount { get; set; }

    public virtual ICollection<Qbtransaction> Qbtransactions { get; set; } = new List<Qbtransaction>();

    public virtual ICollection<ScheduleRevision> ScheduleRevisions { get; set; } = new List<ScheduleRevision>();
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class EstimateCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Sequence { get; set; }

    public Guid? ParentEstimateCategoryId { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<EstimateMapping> EstimateMappings { get; set; } = new List<EstimateMapping>();

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();

    public virtual ICollection<EstimateCategory> InverseParentEstimateCategory { get; set; } = new List<EstimateCategory>();

    public virtual EstimateCategory? ParentEstimateCategory { get; set; }

    public virtual ICollection<ProposalLine> ProposalLineEstimateCategories { get; set; } = new List<ProposalLine>();

    public virtual ICollection<ProposalLine> ProposalLineParentEstimateCategories { get; set; } = new List<ProposalLine>();

    public virtual ICollection<ScheduleTaskMapping> ScheduleTaskMappings { get; set; } = new List<ScheduleTaskMapping>();
}

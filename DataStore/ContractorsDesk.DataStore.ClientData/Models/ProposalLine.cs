using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalLine
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public Guid ProposalId { get; set; }

    public Guid EstimateCategoryId { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public Guid? ParentEstimateCategoryId { get; set; }

    public decimal? SqFoot { get; set; }

    public decimal? Multiplier { get; set; }

    public double? Percentage { get; set; }

    public int? Sequence { get; set; }

    public bool SqFootLocked { get; set; }

    public virtual EstimateCategory EstimateCategory { get; set; } = null!;

    public virtual EstimateCategory? ParentEstimateCategory { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectManagementLine
{
    public Guid Id { get; set; }

    public Guid ProjectManagementId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid? Pred1Id { get; set; }

    public Guid? Pred1LagId { get; set; }

    public Guid? Pred2Id { get; set; }

    public Guid? Pred2LagId { get; set; }

    public Guid? Pred3Id { get; set; }

    public Guid? Pred3LagId { get; set; }

    public decimal ProgressPercentage { get; set; }

    public string Notes { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public int? Duration { get; set; }

    public int Sequence { get; set; }
}

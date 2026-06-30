using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectManagement
{
    public Guid ProjectManagementId { get; set; }

    public Guid QbclassId { get; set; }

    public Guid ConstructionTaskId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public Guid? AssignedTo { get; set; }

    public Guid? Pred1Id { get; set; }

    public Guid? Pred1LagId { get; set; }

    public Guid? Pred2Id { get; set; }

    public Guid? Pred2LagId { get; set; }

    public Guid? Pred3Id { get; set; }

    public Guid? Pred3LagId { get; set; }

    public decimal ProgressPercentage { get; set; }

    public string Notes { get; set; } = null!;

    public string Description { get; set; } = null!;
}

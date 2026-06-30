using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ScheduleTaskMapping
{
    public Guid Id { get; set; }

    public Guid EstimateCategoryId { get; set; }

    public Guid ConstructionTaskId { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ConstructionTask ConstructionTask { get; set; } = null!;

    public virtual EstimateCategory EstimateCategory { get; set; } = null!;
}

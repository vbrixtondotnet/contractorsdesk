using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ConstructionTask
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Sequence { get; set; }

    public Guid? ParentTaskId { get; set; }

    public int Duration { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public Guid? QbclassId { get; set; }

    public Guid? Pred1Id { get; set; }

    public int Pred1Lag { get; set; }

    public Guid? Pred2Id { get; set; }

    public int? Pred2Lag { get; set; }

    public Guid? Pred3Id { get; set; }

    public int? Pred3Lag { get; set; }

    public virtual ICollection<ConstructionTask> InverseParentTask { get; set; } = new List<ConstructionTask>();

    public virtual ConstructionTask? ParentTask { get; set; }

    public virtual Qbclass? Qbclass { get; set; }

    public virtual ICollection<ScheduleTaskMapping> ScheduleTaskMappings { get; set; } = new List<ScheduleTaskMapping>();
}

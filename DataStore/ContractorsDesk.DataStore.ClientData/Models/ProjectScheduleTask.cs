using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectScheduleTask
{
    public Guid Id { get; set; }

    public Guid ProjectScheduleId { get; set; }

    public Guid ConstructionTaskId { get; set; }

    public string Name { get; set; } = null!;

    public int Sequence { get; set; }

    public int Duration { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid? Pred1 { get; set; }

    public int? Lag1 { get; set; }

    public Guid? Pred2 { get; set; }

    public int? Lag2 { get; set; }

    public Guid? Pred3 { get; set; }

    public int? Lag3 { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ProjectSchedule ProjectSchedule { get; set; } = null!;

    public virtual ICollection<ProjectScheduleDelay> ProjectScheduleDelays { get; set; } = new List<ProjectScheduleDelay>();
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectScheduleDelay
{
    public Guid Id { get; set; }

    public Guid ProjectScheduleId { get; set; }

    public Guid? TaskId { get; set; }

    public DateOnly Start { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public int Days { get; set; }

    public bool? ApplyToOtherProjects { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ProjectSchedule ProjectSchedule { get; set; } = null!;

    public virtual ProjectScheduleTask? Task { get; set; }
}

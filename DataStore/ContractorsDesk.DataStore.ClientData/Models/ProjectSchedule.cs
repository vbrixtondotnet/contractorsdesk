using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectSchedule
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public string? Status { get; set; }

    public DateOnly StartDate { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual Qbclass? Project { get; set; }

    public virtual ICollection<ProjectScheduleDelay> ProjectScheduleDelays { get; set; } = new List<ProjectScheduleDelay>();

    public virtual ICollection<ProjectScheduleTask> ProjectScheduleTasks { get; set; } = new List<ProjectScheduleTask>();
}

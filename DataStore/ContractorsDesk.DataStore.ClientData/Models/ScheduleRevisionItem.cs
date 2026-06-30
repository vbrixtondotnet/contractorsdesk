using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ScheduleRevisionItem
{
    public Guid Id { get; set; }

    public Guid ScheduleRevisionId { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public Guid ConstructionTaskId { get; set; }

    public int OldDuration { get; set; }

    public int NewDuration { get; set; }

    public DateOnly OldStartDate { get; set; }

    public DateOnly NewStartDate { get; set; }

    public DateOnly OldEndDate { get; set; }

    public DateOnly NewEndDate { get; set; }

    public virtual ScheduleRevision ScheduleRevision { get; set; } = null!;
}

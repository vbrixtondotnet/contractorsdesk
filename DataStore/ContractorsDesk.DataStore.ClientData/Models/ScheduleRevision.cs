using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ScheduleRevision
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public int? ActionItemId { get; set; }

    public int RevisionNumber { get; set; }

    public DateTime RevisionDate { get; set; }

    public int StatusId { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Qbclass Project { get; set; } = null!;

    public virtual ICollection<ScheduleRevisionItem> ScheduleRevisionItems { get; set; } = new List<ScheduleRevisionItem>();
}

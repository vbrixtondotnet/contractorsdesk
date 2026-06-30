using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectJournal
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public int? CurrentWeek { get; set; }

    public string Journal { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Qbclass Project { get; set; } = null!;
}

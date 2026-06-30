using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectThreshold
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public decimal Threshold { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual Qbclass Project { get; set; } = null!;
}

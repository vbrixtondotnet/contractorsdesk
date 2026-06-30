using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectSupervisor
{
    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public int SupervisorId { get; set; }

    public DateOnly DateAssigned { get; set; }

    public int? SupervisorTypeId { get; set; }

    public virtual Qbclass Project { get; set; } = null!;
}

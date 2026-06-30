using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionItemsSupervisor
{
    public Guid Id { get; set; }

    public int ActionItemId { get; set; }

    public int SupervisorId { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ActionItem ActionItem { get; set; } = null!;

    public virtual User Supervisor { get; set; } = null!;
}

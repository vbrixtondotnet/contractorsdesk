using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalSupervisor
{
    public Guid Id { get; set; }

    public Guid ProposalId { get; set; }

    public int UserId { get; set; }

    public int? SupervisorTypeId { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

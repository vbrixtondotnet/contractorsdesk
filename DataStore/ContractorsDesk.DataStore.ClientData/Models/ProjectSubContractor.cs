using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectSubContractor
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid SubContractorId { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Qbclass Project { get; set; } = null!;

    public virtual SubContractor SubContractor { get; set; } = null!;
}

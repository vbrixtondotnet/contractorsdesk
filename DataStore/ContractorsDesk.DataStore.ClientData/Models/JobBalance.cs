using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class JobBalance
{
    public Guid Id { get; set; }

    public Guid? JobId { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual Qbclass? Job { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwActiveJobs2
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal JobBalance { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwActiveConstructionJobDetail
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Parent { get; set; }

    public string? AccountType { get; set; }

    public decimal? Amount { get; set; }

    public string? ProjectName { get; set; }
}

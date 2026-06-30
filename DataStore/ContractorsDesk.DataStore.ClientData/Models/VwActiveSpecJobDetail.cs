using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwActiveSpecJobDetail
{
    public Guid AccountId { get; set; }

    public string? ParentId { get; set; }

    public string? OriginalName { get; set; }

    public string? MappingName { get; set; }

    public string? AccountType { get; set; }

    public decimal? Amount { get; set; }

    public string? ProjectName { get; set; }
}

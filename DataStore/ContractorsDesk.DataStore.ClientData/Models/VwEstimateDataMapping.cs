using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwEstimateDataMapping
{
    public Guid? MappingId { get; set; }

    public Guid AccountId { get; set; }

    public string? FullyQualifiedName { get; set; }

    public Guid? EstimateCategoryId { get; set; }

    public string? EstimateCategory { get; set; }

    public string? ParentCategory { get; set; }
}

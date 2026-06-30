using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class EstimateSummary
{
    public Guid? EstimateCategoryId { get; set; }

    public Guid? ParentEstimateCategoryId { get; set; }

    public string? Item { get; set; }

    public int? Level { get; set; }

    public decimal? TotalToDate { get; set; }

    public decimal? EstimateAmount { get; set; }

    public decimal? RevisedEstimateAmount { get; set; }

    public decimal? Balance { get; set; }

    public decimal? Percentage { get; set; }
}

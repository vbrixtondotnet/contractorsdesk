using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Estimate
{
    public Guid Id { get; set; }

    public decimal? Amount { get; set; }

    public Guid? EstimateSubCategoryId { get; set; }

    public Guid? QbclassId { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ProjectCompletionDate { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<EstimateHistory> EstimateHistories { get; set; } = new List<EstimateHistory>();

    public virtual EstimateCategory? EstimateSubCategory { get; set; }

    public virtual Qbclass? Qbclass { get; set; }
}

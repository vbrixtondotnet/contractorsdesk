using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class EstimateMapping
{
    public Guid Id { get; set; }

    public string? AccountType { get; set; }

    public string? AccountSubType { get; set; }

    public string? Description { get; set; }

    public Guid? QbaccountId { get; set; }

    public Guid? EstimateSubCategoryId { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public Guid? QbclassId { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual EstimateCategory? EstimateSubCategory { get; set; }

    public virtual Qbaccount? Qbaccount { get; set; }

    public virtual Qbclass? Qbclass { get; set; }
}

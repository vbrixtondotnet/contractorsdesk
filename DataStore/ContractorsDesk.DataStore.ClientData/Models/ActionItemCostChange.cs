using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionItemCostChange
{
    public int Id { get; set; }

    public int? ActionItemId { get; set; }

    public decimal? Amount { get; set; }

    public Guid? EstimateCategoryId { get; set; }

    public bool? RequiresClientApproval { get; set; }

    public virtual ActionItem? ActionItem { get; set; }
}

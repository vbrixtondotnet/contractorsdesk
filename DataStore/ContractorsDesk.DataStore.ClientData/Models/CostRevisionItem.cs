using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class CostRevisionItem
{
    public Guid Id { get; set; }

    public Guid CostRevisionId { get; set; }

    public Guid EstimateCategoryId { get; set; }

    public decimal Amount { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal NewAmount { get; set; }

    public virtual CostRevision CostRevision { get; set; } = null!;
}

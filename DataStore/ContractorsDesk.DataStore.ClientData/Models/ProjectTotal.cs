using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectTotal
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public decimal? MinimumRequestedAmount { get; set; }

    public decimal CostToDate { get; set; }

    public decimal OwnerDeposits { get; set; }

    public DateTime DateUpdated { get; set; }
}

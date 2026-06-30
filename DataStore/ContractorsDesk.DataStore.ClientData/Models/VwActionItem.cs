using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwActionItem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ActionTypeId { get; set; }

    public string ActionTypeName { get; set; } = null!;

    public Guid ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public int Status { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid? CostChangeItemId { get; set; }

    public string? CostChangeItem { get; set; }

    public decimal? Amount { get; set; }

    public Guid? ScheduleChangeItemId { get; set; }

    public string? ScheduleChangeItem { get; set; }

    public int? NoOfDays { get; set; }
}

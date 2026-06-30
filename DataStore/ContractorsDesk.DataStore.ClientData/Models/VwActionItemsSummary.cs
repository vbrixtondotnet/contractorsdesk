using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwActionItemsSummary
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly DueDate { get; set; }

    public int ActionTypeId { get; set; }

    public int Source { get; set; }

    public string ActionTypeName { get; set; } = null!;

    public int StatusId { get; set; }

    public bool IsArchived { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid? ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public Guid? CostChangeItemId { get; set; }

    public string? CostChangeItem { get; set; }

    public decimal? Amount { get; set; }

    public decimal? CurrentAmount { get; set; }

    public Guid? ScheduleChangeItemId { get; set; }

    public string? ScheduleChangeItem { get; set; }

    public int? NoOfDays { get; set; }

    public int? Duration { get; set; }

    public int CreatedById { get; set; }

    public string CreatedBy { get; set; } = null!;
}

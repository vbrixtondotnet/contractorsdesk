using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalLinesHistory
{
    public Guid HistoryId { get; set; }

    public Guid ProposalLineId { get; set; }

    public Guid ProposalId { get; set; }

    public string ChangeType { get; set; } = null!;

    public DateTime ChangeDate { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public Guid EstimateCategoryId { get; set; }

    public Guid? ParentEstimateCategoryId { get; set; }

    public DateTime? Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? Updated { get; set; }

    public string? UpdatedBy { get; set; }

    public double? Percentage { get; set; }
}

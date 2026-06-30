using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbcreditMemo
{
    public string Id { get; set; } = null!;

    public string? DocNumber { get; set; }

    public string? CustomerName { get; set; }

    public decimal? RemainingCredit { get; set; }

    public decimal? TotalTax { get; set; }

    public decimal? TotalAmt { get; set; }

    public bool? ApplyTaxAfterDiscount { get; set; }

    public decimal? Balance { get; set; }

    public string? Currency { get; set; }

    public DateTime? TxnDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

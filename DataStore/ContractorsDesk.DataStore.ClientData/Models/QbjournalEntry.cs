using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbjournalEntry
{
    public string Id { get; set; } = null!;

    public DateTime? TxnDate { get; set; }

    public string? RefNumber { get; set; }

    public string? DebitAccountRef { get; set; }

    public decimal? DebitAccountAmount { get; set; }

    public string? DebitEntityRef { get; set; }

    public string? CreditAccountRef { get; set; }

    public decimal? CreditAccountAmount { get; set; }

    public string? CreditEntityRef { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

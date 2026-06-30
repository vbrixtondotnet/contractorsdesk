using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbvendorCredit
{
    public string Id { get; set; } = null!;

    public string? VendorName { get; set; }

    public string? ApaccountName { get; set; }

    public decimal? Balance { get; set; }

    public decimal? TotalAmt { get; set; }

    public DateTime? TxnDate { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

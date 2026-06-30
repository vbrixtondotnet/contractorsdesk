using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Test1
{
    public string? AccountName { get; set; }

    public decimal? Amount { get; set; }

    public string? TxnNumber { get; set; }

    public string? Memo { get; set; }

    public string? Class { get; set; }

    public string? Vendor { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class MismatchesJobTransaction
{
    public string? BankingAccountApp { get; set; }

    public string? BankingAccountQbo { get; set; }

    public string? AccountCategory { get; set; }

    public string? Class { get; set; }

    public DateOnly? Date { get; set; }

    public string? TxnId { get; set; }

    public string? RefNo { get; set; }

    public string? Type { get; set; }

    public string? Payee { get; set; }

    public string? Memo { get; set; }

    public string? DivisionLocation { get; set; }

    public decimal? Amount { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbtransactionsDeleted
{
    public Guid Id { get; set; }

    public DateTime? DateDeleted { get; set; }

    public Guid? AccountId { get; set; }

    public decimal? Amount { get; set; }

    public Guid? VendorId { get; set; }

    public Guid? ClassId { get; set; }

    public Guid? CustomerId { get; set; }

    public string? TxnId { get; set; }

    public string? TxnNumber { get; set; }

    public string? TxnType { get; set; }

    public string? Memo { get; set; }

    public string? Name { get; set; }

    public string? Currency { get; set; }

    public string? Status { get; set; }

    public string? IsCleared { get; set; }

    public decimal? ExchangeRate { get; set; }

    public DateTime? TransactionDate { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Updated { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public Guid? SplitAccountId { get; set; }

    public string? Location { get; set; }

    public virtual Qbaccount? Account { get; set; }

    public virtual Qbclass? Class { get; set; }

    public virtual Qbcustomer? Customer { get; set; }

    public virtual Qbvendor? Vendor { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qbinvoice
{
    public string Id { get; set; } = null!;

    public decimal? Deposit { get; set; }

    public string? InvoiceStatus { get; set; }

    public string? EinvoiceStatus { get; set; }

    public string? CustomerName { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal? TotalAmt { get; set; }

    public bool? ApplyTaxAfterDiscount { get; set; }

    public bool? ShippingTaxIncludedInTotalTax { get; set; }

    public decimal? Balance { get; set; }

    public string? PaymentType { get; set; }

    public string? DocNumber { get; set; }

    public decimal? NetAmount0 { get; set; }

    public decimal? TaxAmount8 { get; set; }

    public decimal? NetAmount8 { get; set; }

    public decimal? TaxAmount18 { get; set; }

    public decimal? NetAmount18 { get; set; }

    public decimal? TotalTax { get; set; }

    public DateTime? TxnDate { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

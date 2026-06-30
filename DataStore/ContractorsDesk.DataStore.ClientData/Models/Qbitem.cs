using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qbitem
{
    public Guid Id { get; set; }

    public string? ListId { get; set; }

    public string? Name { get; set; }

    public string? FullName { get; set; }

    public string? ExpenseAccountId { get; set; }

    public string? TaxCode { get; set; }

    public string? TaxRate { get; set; }

    public bool? IsActive { get; set; }

    public string? Type { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

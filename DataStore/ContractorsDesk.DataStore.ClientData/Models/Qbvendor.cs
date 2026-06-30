using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qbvendor
{
    public Guid Id { get; set; }

    public string ListId { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? CompanyName { get; set; }

    public string? Address { get; set; }

    public string? Nace { get; set; }

    public string? Vatnumber { get; set; }

    public string? Nuinumber { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Qbtransaction> Qbtransactions { get; set; } = new List<Qbtransaction>();
}

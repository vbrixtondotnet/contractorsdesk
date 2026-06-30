using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qbcustomer
{
    public Guid Id { get; set; }

    public string ListId { get; set; } = null!;

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? FullName { get; set; }

    public string? CompanyName { get; set; }

    public string? Address { get; set; }

    public string? Nace { get; set; }

    public decimal? Balance { get; set; }

    public string? Currency { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? SecondaryEmail { get; set; }

    public bool? IsActive { get; set; }

    public int? Level { get; set; }

    public string? ParentId { get; set; }

    public string? JobStatus { get; set; }

    public DateTime? JobStartDate { get; set; }

    public DateTime? JobProjectedEndDate { get; set; }

    public DateTime? JobEndDate { get; set; }

    public string? JobDesc { get; set; }

    public string? Vatnumber { get; set; }

    public string? Nuinumber { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public virtual ICollection<Qbtransaction> Qbtransactions { get; set; } = new List<Qbtransaction>();
}

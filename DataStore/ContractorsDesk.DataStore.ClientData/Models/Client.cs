using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Client
{
    public Guid Id { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? CompanyName { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? SecondaryEmailAddress { get; set; }

    public string? Phone { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();

    public virtual User? User { get; set; }
}

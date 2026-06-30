using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Vendor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Zip { get; set; }

    public string? Category { get; set; }

    public string? Company { get; set; }
}

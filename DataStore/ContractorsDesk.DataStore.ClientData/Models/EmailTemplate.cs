using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class EmailTemplate
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string EmailType { get; set; } = null!;

    public string Body { get; set; } = null!;

    public int OwnerId { get; set; }

    public bool IsDefault { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedById { get; set; }

    public DateTime DateModified { get; set; }

    public int ModifiedById { get; set; }
}

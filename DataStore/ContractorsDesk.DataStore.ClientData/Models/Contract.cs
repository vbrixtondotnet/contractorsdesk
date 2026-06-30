using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Contract
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string BodyTemplate { get; set; } = null!;

    public string DefaultFolderName { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int CreatedById { get; set; }

    public DateTime DateModified { get; set; }

    public int ModifiedById { get; set; }
}

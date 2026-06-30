using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Qblocation
{
    public Guid Id { get; set; }

    public string ListId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }
}

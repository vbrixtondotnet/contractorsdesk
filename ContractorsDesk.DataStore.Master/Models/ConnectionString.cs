using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Master.Models;

public partial class ConnectionString
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}

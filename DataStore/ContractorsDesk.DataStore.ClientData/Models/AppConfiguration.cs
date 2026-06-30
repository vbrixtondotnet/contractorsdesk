using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class AppConfiguration
{
    public Guid Id { get; set; }

    public string? Key { get; set; }

    public DateTime? Value { get; set; }
}

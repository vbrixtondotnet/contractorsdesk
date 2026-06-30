using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SpErrorLog
{
    public int Id { get; set; }

    public string SpName { get; set; } = null!;

    public string ErrorMessage { get; set; } = null!;

    public DateTime DateCreated { get; set; }
}

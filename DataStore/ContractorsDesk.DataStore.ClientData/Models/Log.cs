using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Log
{
    public int Id { get; set; }

    public string Timestamp { get; set; } = null!;

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Exception { get; set; } = null!;

    public string Properties { get; set; } = null!;

    public DateTime? Ts { get; set; }
}

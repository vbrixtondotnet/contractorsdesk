using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SysBackgroundJob
{
    public int SysJobsId { get; set; }

    public string JobName { get; set; } = null!;

    public DateTime DateOfExecution { get; set; }

    public string LastStatus { get; set; } = null!;

    public string? ApiEndpoint { get; set; }

    public bool? Development { get; set; }

    public bool? Staging { get; set; }

    public bool? Production { get; set; }
}

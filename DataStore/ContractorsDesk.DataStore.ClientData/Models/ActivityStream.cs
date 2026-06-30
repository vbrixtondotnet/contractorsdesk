using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActivityStream
{
    public int Id { get; set; }

    public string Ref { get; set; } = null!;

    public string? StepName { get; set; }

    public string? Reason { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }
}

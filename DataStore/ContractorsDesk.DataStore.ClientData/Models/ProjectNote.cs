using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectNote
{
    public Guid Id { get; set; }

    public Guid? ProjectId { get; set; }

    public string? Notes { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Qbclass? Project { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}

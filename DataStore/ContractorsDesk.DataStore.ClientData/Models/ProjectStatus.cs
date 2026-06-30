using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectStatus
{
    public Guid Id { get; set; }

    public string ProjectName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public int Progress { get; set; }

    public Guid? ProjectManagementId { get; set; }
}

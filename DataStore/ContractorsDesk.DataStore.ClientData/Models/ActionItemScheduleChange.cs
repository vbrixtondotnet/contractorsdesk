using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionItemScheduleChange
{
    public int Id { get; set; }

    public int? ActionItemId { get; set; }

    public int? NoOfDays { get; set; }

    public Guid? ConstructionTaskId { get; set; }

    public bool? RequiresClientApproval { get; set; }

    public virtual ActionItem? ActionItem { get; set; }
}

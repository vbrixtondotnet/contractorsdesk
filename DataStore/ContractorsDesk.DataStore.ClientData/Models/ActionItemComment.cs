using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionItemComment
{
    public int Id { get; set; }

    public int? ActionItemId { get; set; }

    public string? Comment { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ActionItem? ActionItem { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();
}

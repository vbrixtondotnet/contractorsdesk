using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ActionItem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? ProjectId { get; set; }

    public int ActionTypeId { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateOnly DueDate { get; set; }

    public int Status { get; set; }

    public bool IsArchived { get; set; }

    public int? AcceptedBy { get; set; }

    public int Source { get; set; }

    public virtual User? AcceptedByNavigation { get; set; }

    public virtual ICollection<ActionItemComment> ActionItemComments { get; set; } = new List<ActionItemComment>();

    public virtual ICollection<ActionItemCostChange> ActionItemCostChanges { get; set; } = new List<ActionItemCostChange>();

    public virtual ICollection<ActionItemScheduleChange> ActionItemScheduleChanges { get; set; } = new List<ActionItemScheduleChange>();

    public virtual ICollection<ActionItemsSupervisor> ActionItemsSupervisors { get; set; } = new List<ActionItemsSupervisor>();

    public virtual ActionType ActionType { get; set; } = null!;

    public virtual ICollection<ChangeOrder> ChangeOrders { get; set; } = new List<ChangeOrder>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Qbclass? Project { get; set; }
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool RequireLogin { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public bool IsDeleted { get; set; }

    public int Status { get; set; }

    public string? ConfirmationCode { get; set; }

    public virtual ICollection<ActionItem> ActionItemAcceptedByNavigations { get; set; } = new List<ActionItem>();

    public virtual ICollection<ActionItemComment> ActionItemComments { get; set; } = new List<ActionItemComment>();

    public virtual ICollection<ActionItem> ActionItemCreatedByNavigations { get; set; } = new List<ActionItem>();

    public virtual ICollection<ActionItemsSupervisor> ActionItemsSupervisors { get; set; } = new List<ActionItemsSupervisor>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<ProjectNote> ProjectNoteCreatedByNavigations { get; set; } = new List<ProjectNote>();

    public virtual ICollection<ProjectNote> ProjectNoteUpdatedByNavigations { get; set; } = new List<ProjectNote>();

    public virtual ICollection<Proposal> ProposalCreatedByNavigations { get; set; } = new List<Proposal>();

    public virtual ICollection<ProposalSupervisor> ProposalSupervisors { get; set; } = new List<ProposalSupervisor>();

    public virtual ICollection<Proposal> ProposalUpdatedByNavigations { get; set; } = new List<Proposal>();

    public virtual Role Role { get; set; } = null!;
}

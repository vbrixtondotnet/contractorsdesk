using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Proposal
{
    public Guid Id { get; set; }

    public int Number { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid? QbclassId { get; set; }

    public Guid? QbcustomerId { get; set; }

    public Guid? ProposalProjectId { get; set; }

    public Guid? ClientId { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalAmount { get; set; }

    public string DocStatus { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsArchived { get; set; }

    public bool IncludeLinesWithZeroAmount { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Client? Client { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ProposalLine> ProposalLines { get; set; } = new List<ProposalLine>();

    public virtual ProposalProject? ProposalProject { get; set; }

    public virtual ICollection<ProposalSupervisor> ProposalSupervisors { get; set; } = new List<ProposalSupervisor>();

    public virtual Qbclass? Qbclass { get; set; }

    public virtual ProposalTemplate? Template { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}

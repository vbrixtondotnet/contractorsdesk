using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalTemplate
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ICollection<ProposalTemplateUserDefault> ProposalTemplateUserDefaults { get; set; } = new List<ProposalTemplateUserDefault>();

    public virtual ICollection<ProposalTemplatesLineItem> ProposalTemplatesLineItems { get; set; } = new List<ProposalTemplatesLineItem>();

    public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}

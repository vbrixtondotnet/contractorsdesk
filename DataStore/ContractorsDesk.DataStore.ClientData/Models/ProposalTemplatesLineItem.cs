using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalTemplatesLineItem
{
    public Guid Id { get; set; }

    public Guid ProposalTemplateId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Amount { get; set; }

    public double? Percentage { get; set; }

    public int Sequence { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? EstimateCategoryId { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public virtual ICollection<ProposalTemplatesLineItem> InverseParent { get; set; } = new List<ProposalTemplatesLineItem>();

    public virtual ProposalTemplatesLineItem? Parent { get; set; }

    public virtual ProposalTemplate ProposalTemplate { get; set; } = null!;
}

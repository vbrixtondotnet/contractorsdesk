using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProposalTemplateUserDefault
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public Guid TemplateId { get; set; }

    public virtual ProposalTemplate Template { get; set; } = null!;
}

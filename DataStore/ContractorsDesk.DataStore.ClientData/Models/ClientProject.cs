using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ClientProject
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public Guid ProjectId { get; set; }

    public virtual Qbclass Project { get; set; } = null!;
}

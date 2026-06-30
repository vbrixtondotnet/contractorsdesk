using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class UserLog
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }
}

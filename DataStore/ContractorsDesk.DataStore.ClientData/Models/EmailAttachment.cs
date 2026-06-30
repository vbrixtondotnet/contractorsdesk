using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class EmailAttachment
{
    public int Id { get; set; }

    public Guid EmailId { get; set; }

    public string? FileName { get; set; }

    public string? FileUrl { get; set; }

    public virtual Email Email { get; set; } = null!;
}

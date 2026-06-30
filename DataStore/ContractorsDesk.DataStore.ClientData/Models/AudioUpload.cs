using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class AudioUpload
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? StorageStatus { get; set; }

    public string? Aistatus { get; set; }

    public string? Transcript { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual Qbclass? Project { get; set; }
}

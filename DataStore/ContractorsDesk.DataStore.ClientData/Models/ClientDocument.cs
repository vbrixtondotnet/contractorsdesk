using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ClientDocument
{
    public Guid Id { get; set; }

    public Guid FolderId { get; set; }

    public Guid ClientId { get; set; }

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public int Version { get; set; }

    public string Url { get; set; } = null!;

    public Guid? SubcontractorId { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual SysFolder Folder { get; set; } = null!;

    public virtual SubContractor? Subcontractor { get; set; }
}

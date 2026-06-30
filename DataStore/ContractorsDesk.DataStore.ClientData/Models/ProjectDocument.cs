using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ProjectDocument
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public int DocumentTypeId { get; set; }

    public string FileUrl { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Qbclass Project { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SysFolder
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public int Sequence { get; set; }

    public DateTime DateCreated { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ClientDocument> ClientDocuments { get; set; } = new List<ClientDocument>();
}

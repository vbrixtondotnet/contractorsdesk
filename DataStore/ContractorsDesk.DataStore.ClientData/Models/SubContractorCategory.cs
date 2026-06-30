using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SubContractorCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; }
}

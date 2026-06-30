using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbolastModifiedTimestamp
{
    public string Id { get; set; } = null!;

    public string RealmId { get; set; } = null!;

    public DateTime LastModified { get; set; }
}

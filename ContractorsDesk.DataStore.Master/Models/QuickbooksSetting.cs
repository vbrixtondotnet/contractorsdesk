using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Master.Models;

public partial class QuickbooksSetting
{
    public Guid Id { get; set; }

    public Guid? CompanyId { get; set; }

    public string RealmId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public virtual Company? Company { get; set; }
}

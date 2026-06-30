using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class Token
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public string RealmId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;
}

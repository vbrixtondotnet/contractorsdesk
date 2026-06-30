using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Master.Models;

public partial class Company
{
    public Guid Id { get; set; }

    public string? SubDomain { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ConnectionString> ConnectionStrings { get; set; } = new List<ConnectionString>();

    public virtual ICollection<QuickbooksSetting> QuickbooksSettings { get; set; } = new List<QuickbooksSetting>();
}

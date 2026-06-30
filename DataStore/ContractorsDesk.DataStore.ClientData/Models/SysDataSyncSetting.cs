using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SysDataSyncSetting
{
    public Guid Id { get; set; }

    public string? DataSyncName { get; set; }

    public bool? IsFirstRun { get; set; }

    public int? NumberOfDaysLookup { get; set; }

    public DateTime? DateLastRun { get; set; }

    public DateTime? DateNextRun { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }
}

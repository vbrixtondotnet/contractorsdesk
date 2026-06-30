using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbclassesExcelNew
{
    public string? ClassId { get; set; }

    public Guid? QbaccountId { get; set; }

    public bool? ActiveJobs { get; set; }

    public bool? AllowedForJobReports { get; set; }

    public bool? OpenJob { get; set; }

    public DateTime? OpenedDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string? Ownership { get; set; }
}

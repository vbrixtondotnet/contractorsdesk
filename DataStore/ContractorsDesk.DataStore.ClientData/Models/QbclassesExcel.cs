using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class QbclassesExcel
{
    public string? FullyQualifiedName { get; set; }

    public Guid? QbaccountId { get; set; }

    public bool? AllowedForJobReports { get; set; }

    public bool? OpenJob { get; set; }

    public DateTime? ClosedDate { get; set; }
}

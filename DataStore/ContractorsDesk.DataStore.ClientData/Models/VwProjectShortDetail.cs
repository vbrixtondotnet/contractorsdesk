using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwProjectShortDetail
{
    public Guid Id { get; set; }

    public Guid? ProposalId { get; set; }

    public string? Name { get; set; }

    public string? FullyQualifiedName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? JobBalance { get; set; }

    public bool IsArchived { get; set; }

    public string? DocStatus { get; set; }

    public int? SupervisorId { get; set; }

    public string? SupervisorFirstName { get; set; }

    public string? SupervisorLastName { get; set; }

    public string? SupervisorEmail { get; set; }

    public string? ClientEmailAddress { get; set; }

    public string? ClientName { get; set; }

    public bool? IsActive { get; set; }
}

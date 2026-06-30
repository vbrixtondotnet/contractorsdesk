using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class VwSupervisorAndClientActiveJob
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal JobBalance { get; set; }

    public decimal Threshold { get; set; }

    public bool IsArchived { get; set; }

    public int? SupervisorId { get; set; }

    public string? SupervisorFirstName { get; set; }

    public string? SupervisorLastName { get; set; }

    public int? ClientId { get; set; }

    public string? ClientFirstName { get; set; }

    public string? ClientLastName { get; set; }

    public string? ClientFullName { get; set; }

    public string? ClientEmailAddress { get; set; }

    public Guid? ProposalId { get; set; }

    public string? DocStatus { get; set; }

    public bool IsCompleted { get; set; }

    public string? PhotoUrl { get; set; }
}

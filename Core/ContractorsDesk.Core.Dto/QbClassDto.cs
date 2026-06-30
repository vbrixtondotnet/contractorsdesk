using System;
using System.Collections.Generic;

namespace ContractorsDesk.Core.Dto;

public partial class QbClassDto
{
    public Guid Id { get; set; }

    public string? ListId { get; set; }

    public string? Name { get; set; }

    public string? FullyQualifiedName { get; set; }

    public string? Address { get; set; }

    public bool? SubClass { get; set; }

    public string? ParentId { get; set; }

    public DateTime? TimeCreated { get; set; }

    public DateTime? TimeModified { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? AllowedForJobReports { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string? Notes { get; set; }

    public bool? OpenJob { get; set; }

    public Guid? QbaccountId { get; set; }

    public DateTime? OpenedDate { get; set; }

    public string? Ownership { get; set; }

    public bool? ActiveJobs { get; set; }

    public bool? ActiveSpecJobs { get; set; }

    public bool? AllowedForBudgetReports { get; set; }

    public string? Description { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public bool IsArchived { get; set; }

    public bool IsDeleted { get; set; }

   
}

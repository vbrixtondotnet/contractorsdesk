using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class SubContractor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Company { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? LicenseNo { get; set; }

    public DateOnly? LicenseExp { get; set; }

    public string? LiabilityInsurancePolicyNo { get; set; }

    public DateOnly? LiabilityInsuranceExpiry { get; set; }

    public string? CompInsurancePolicyNo { get; set; }

    public DateOnly? CompInsuranceExpiry { get; set; }

    public string? BondInsurancePolicyNo { get; set; }

    public DateOnly? BondInsuranceExpiry { get; set; }

    public string? Category { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public bool? IsActive { get; set; }

    public string? Zip { get; set; }

    public virtual ICollection<ClientDocument> ClientDocuments { get; set; } = new List<ClientDocument>();

    public virtual ICollection<ProjectSubContractor> ProjectSubContractors { get; set; } = new List<ProjectSubContractor>();
}

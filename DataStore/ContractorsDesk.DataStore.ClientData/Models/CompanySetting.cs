using System;
using System.Collections.Generic;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class CompanySetting
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyEmail { get; set; }

    public string? GeneralContractorName { get; set; }

    public string? CompanyLogoUrl { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? PhoneNumber { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Zip { get; set; }
}
